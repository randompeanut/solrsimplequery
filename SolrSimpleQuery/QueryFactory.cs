using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolrSimpleQuery.Filters;
using SolrSimpleQuery.Filters.Interfaces;
using SolrSimpleQuery.Models;
using SolrSimpleQuery.Utility.Extensions;
using SolrSimpleQuery.Utility.Formatters;

namespace SolrSimpleQuery
{
    public interface IQueryFactory
    {
        SolrResponse<List<TResult>> Query<TResult>(FilterCriteria filterCriteria);

        string[] QueryCsv(FilterCriteria filterCriteria);

        string QueryRaw(FilterCriteria filterCriteria);

        SolrResponse<List<List<DynamicResult>>> QueryDynamic(FilterCriteria filterCriteria);

        SolrResponse<Dictionary<TIdentifierField, TResult>> QueryGrouped<TIdentifierField, TResult>(
            FilterCriteria filterCriteria);

        SolrResponse<Dictionary<TIdentifierField, List<DynamicResult>>> QueryGroupedDynamic<TIdentifierField>(
            FilterCriteria filterCriteria);

        dynamic GetFieldValueByIdentifier<TIdentifierField, TResult>(
            SolrResponse<Dictionary<TIdentifierField, TResult>> resultList,
            TIdentifierField identifierFieldValue,
            string fieldName);

        dynamic GetFieldValueByIdentifier<TIdentifierField>(FilterCriteria filterCriteria, string fieldName);

        TResult GetObjectByIdentifier<TIdentifierField, TResult>(
            SolrResponse<Dictionary<TIdentifierField, TResult>> resultList,
            TIdentifierField identifierFieldValue);

        TResult GetObjectByIdentifier<TIdentifierField, TResult>(FilterCriteria filterCriteria)
            where TResult : class, new();

        List<DynamicResult> GetDynamicObjectByIdentifier<TIdentifierField>(FilterCriteria filterCriteria);
    }

    public class QueryFactory : IQueryFactory
    {
        private static readonly WebClient Client = new WebClient();
        private static QueryFactory _instance;
        private string _baseUrl = ConfigurationManager.AppSettings["SolrSimpleQueryBaseUrl"];
        private string _channel = ConfigurationManager.AppSettings["SolrSimpleQueryChannel"];

        /// <summary>
        ///     Provides a static instance of QueryFactory
        /// </summary>
        public static QueryFactory Instance => _instance ?? (_instance = new QueryFactory());

        public SolrResponse<List<TResult>> Query<TResult>(FilterCriteria filterCriteria)
        {
            var result = DoQuery<SolrResponse<List<TResult>>>(filterCriteria);

            if (filterCriteria.FacetQuery)
                result.Facet_Counts.Facet_Fields = result.Facet_Counts.GetFacetValues();

            return result;
        }

        public string[] QueryCsv(FilterCriteria filterCriteria)
        {
            return DoQueryCsv(filterCriteria);
        }

        public string QueryRaw(FilterCriteria filterCriteria)
        {
            return DoQuery(filterCriteria);
        }

        public SolrResponse<List<List<DynamicResult>>> QueryDynamic(FilterCriteria filterCriteria)
        {
            var requestResponse = new SolrResponse<List<List<DynamicResult>>>();
            var valueCollection = new List<List<DynamicResult>>();

            var result = DoQuery<SolrResponse<List<dynamic>>>(filterCriteria);

            if (filterCriteria.FacetQuery)
                requestResponse.Facet_Counts = new FacetResponse {Facet_Fields = result.Facet_Counts.GetFacetValues()};

            requestResponse.ResponseHeader = result.ResponseHeader;
            requestResponse.Response = new Response<List<List<DynamicResult>>>
            {
                NumFound = result.Response.NumFound,
                Start = result.Response.Start,
                Docs = new List<List<DynamicResult>> {new List<DynamicResult>()}
            };

            foreach (var row in result.Response.Docs)
            {
                var resultPairs = new List<DynamicResult>();

                foreach (var prop in row)
                    try
                    {
                        if (filterCriteria.FieldList != null && filterCriteria.FieldList.Any() &&
                            filterCriteria.FieldList.All(r => r != prop.Name)) continue;

                        var properties = ((JProperty) prop).Value.ToArray();
                        resultPairs.Add(new DynamicResult(prop.Name, properties.Any() ? properties : prop.Value));
                    }
                    catch (RuntimeBinderException)
                    {
                        //TODO: process these child objects
                        //this is a collection and should be handled differently
                    }

                valueCollection.Add(resultPairs);
            }

            requestResponse.Response.Docs = valueCollection;

            return requestResponse;
        }

        public SolrResponse<Dictionary<TIdentifierField, TResult>> QueryGrouped<TIdentifierField, TResult>(
            FilterCriteria filterCriteria)
        {
            var requestResponse = new SolrResponse<Dictionary<TIdentifierField, TResult>>();
            var valueCollection = new Dictionary<TIdentifierField, TResult>();

            var result = DoQuery<SolrResponse<List<TResult>>>(filterCriteria);

            if (filterCriteria.FacetQuery)
                requestResponse.Facet_Counts = new FacetResponse {Facet_Fields = result.Facet_Counts.GetFacetValues()};

            requestResponse.ResponseHeader = result.ResponseHeader;
            requestResponse.Response = new Response<Dictionary<TIdentifierField, TResult>>
            {
                NumFound = result.Response.NumFound,
                Start = result.Response.Start,
                Docs = new Dictionary<TIdentifierField, TResult>()
            };

            var identifierFieldProperty = typeof(TResult).GetProperty(filterCriteria.IdentifierFieldName);
            if (identifierFieldProperty == null) return null;

            foreach (var row in result.Response.Docs)
            {
                var identifierFieldValue = identifierFieldProperty.GetValue(row);
                valueCollection.Add(
                    (TIdentifierField) Convert.ChangeType(identifierFieldValue, typeof(TIdentifierField)), row);
            }

            requestResponse.Response.Docs = valueCollection;

            return requestResponse;
        }

        public SolrResponse<Dictionary<TIdentifierField, List<DynamicResult>>> QueryGroupedDynamic<TIdentifierField>(
            FilterCriteria filterCriteria)
        {
            var requestResponse = new SolrResponse<Dictionary<TIdentifierField, List<DynamicResult>>>();
            var valueCollection = new Dictionary<TIdentifierField, List<DynamicResult>>();

            if (!filterCriteria.FieldList.Contains(filterCriteria.IdentifierFieldName))
            {
                var fieldList = new string[filterCriteria.FieldList.Length + 1];
                Array.Copy(filterCriteria.FieldList, fieldList, filterCriteria.FieldList.Length);

                fieldList[filterCriteria.FieldList.Length] = filterCriteria.IdentifierFieldName;

                filterCriteria.FieldList = fieldList;
            }

            var result = DoQuery<SolrResponse<List<dynamic>>>(filterCriteria);

            if (filterCriteria.FacetQuery)
                requestResponse.Facet_Counts = new FacetResponse {Facet_Fields = result.Facet_Counts.GetFacetValues()};

            requestResponse.ResponseHeader = result.ResponseHeader;
            requestResponse.Response = new Response<Dictionary<TIdentifierField, List<DynamicResult>>>
            {
                NumFound = result.Response.NumFound,
                Start = result.Response.Start,
                Docs = new Dictionary<TIdentifierField, List<DynamicResult>>()
            };

            foreach (var row in result.Response.Docs)
            {
                var identifierFieldValue = default(TIdentifierField);
                var resultPairs = new List<DynamicResult>();

                foreach (var prop in row)
                {
                    if (prop.Name == filterCriteria.IdentifierFieldName)
                        identifierFieldValue =
                            (TIdentifierField) Convert.ChangeType(prop.First.Value, typeof(TIdentifierField));

                    try
                    {
                        if (filterCriteria.FieldList != null && filterCriteria.FieldList.Any() &&
                            filterCriteria.FieldList.All(r => r != prop.Name)) continue;

                        var properties = ((JProperty)prop).Value.ToArray();
                        resultPairs.Add(new DynamicResult(prop.Name, properties.Any() ? properties : prop.Value));
                    }
                    catch (RuntimeBinderException)
                    {
                        //TODO: process these child objects
                        //this is a collection and should be handled differently
                    }
                }

                if (identifierFieldValue != null && !valueCollection.ContainsKey(identifierFieldValue))
                    valueCollection.Add(identifierFieldValue, resultPairs);
            }

            requestResponse.Response.Docs = valueCollection;

            return requestResponse;
        }

        public dynamic GetFieldValueByIdentifier<TIdentifierField, TResult>(
            SolrResponse<Dictionary<TIdentifierField, TResult>> resultList,
            TIdentifierField identifierFieldValue,
            string fieldName)
        {
            return resultList.GetFieldValueByIdentifier(identifierFieldValue, fieldName);
        }

        public dynamic GetFieldValueByIdentifier<TIdentifierField>(FilterCriteria filterCriteria, string fieldName)
        {
            var resultList = QueryGroupedDynamic<TIdentifierField>(filterCriteria);
            return resultList.GetFieldValueByIdentifier(
                    (TIdentifierField) Convert.ChangeType(filterCriteria.IdentifierFieldValue,
                        typeof(TIdentifierField)),
                    fieldName)
                .Value;
        }

        public TResult GetObjectByIdentifier<TIdentifierField, TResult>(
            SolrResponse<Dictionary<TIdentifierField, TResult>> resultList,
            TIdentifierField identifierFieldValue)
        {
            return resultList.GetObjectByIdentifier(identifierFieldValue);
        }

        public TResult GetObjectByIdentifier<TIdentifierField, TResult>(FilterCriteria filterCriteria)
            where TResult : class, new()
        {
            var resultList = QueryGrouped<TIdentifierField, TResult>(filterCriteria);
            return resultList.GetObjectByIdentifier(
                (TIdentifierField) Convert.ChangeType(filterCriteria.IdentifierFieldValue, typeof(TIdentifierField)));
        }

        public List<DynamicResult> GetDynamicObjectByIdentifier<TIdentifierField>(FilterCriteria filterCriteria)
        {
            var resultList = QueryGroupedDynamic<TIdentifierField>(filterCriteria);
            return resultList.GetObjectByIdentifier(
                (TIdentifierField) Convert.ChangeType(filterCriteria.IdentifierFieldValue, typeof(TIdentifierField)));
        }

        public static void SetBaseUrl(string baseUrl)
        {
            Instance._baseUrl = baseUrl;
        }

        public static void SetChannel(string channel)
        {
            Instance._channel = channel;
        }

        /// <summary>
        ///     At the moment this is mainly for unit testing purposes
        /// </summary>
        public static void ResetInstance()
        {
            _instance = null;
        }

        /// <summary>
        ///     Performs a Solr query and returns a list of TResult
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="baseUrl"></param>
        /// <param name="channel"></param>
        /// <param name="fieldList"></param>
        /// <param name="filterCollection"></param>
        /// <param name="sortField"></param>
        /// <param name="sortBy"></param>
        /// <param name="start"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static TResult DoQuery<TResult>(FilterCriteria filterCriteria)
        {
            var resultString = DoQuery(filterCriteria);
            return JsonConvert.DeserializeObject<TResult>(resultString);
        }

        private static string DoQuery(FilterCriteria filterCriteria)
        {
            CheckFilterCriteriaForIdentifierValue(filterCriteria);

            using (Client)
            {
                var resultString = Client.DownloadString(CreateUrl(filterCriteria));

                return resultString;
            }
        }

        /// <summary>
        ///     Performs a Solr query and returns a list of TResult
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="baseUrl"></param>
        /// <param name="channel"></param>
        /// <param name="fieldList"></param>
        /// <param name="filterCollection"></param>
        /// <param name="sortField"></param>
        /// <param name="sortBy"></param>
        /// <param name="start"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static string[] DoQueryCsv(FilterCriteria filterCriteria)
        {
            CheckFilterCriteriaForIdentifierValue(filterCriteria);

            using (Client)
            {
                var result = Client.DownloadString(CreateUrl(filterCriteria)).Split(',').ToList();

                result.Sort();

                return result.ToArray();
            }
        }

        private static void CheckFilterCriteriaForIdentifierValue(FilterCriteria filterCriteria)
        {
            if (filterCriteria.IdentifierFieldName == null || filterCriteria.IdentifierFieldValue == null) return;

            if (filterCriteria.FilterList == null && filterCriteria.UrlFilterList == null)
                filterCriteria.FilterList = new IFilter[0];

            if (filterCriteria.FilterList != null)
            {
                if (filterCriteria.UrlFilterList != null)
                    filterCriteria.UrlFilterList = null; // give preference to IFilter filterList

                if (filterCriteria.FilterList.All(r => r.FieldName != filterCriteria.IdentifierFieldName))
                {
                    var filterList = new IFilter[filterCriteria.FilterList.Length + 1];
                    Array.Copy(filterCriteria.FilterList, filterList, filterCriteria.FilterList.Length);

                    filterList[filterCriteria.FilterList.Length] =
                        new SimpleFilter<string>().Create(filterCriteria.IdentifierFieldName,
                            filterCriteria.IdentifierFieldValue);

                    filterCriteria.FilterList = filterList;
                }
            }

            if (filterCriteria.UrlFilterList != null)
                if (filterCriteria.UrlFilterList.All(r => r.Split('-')[0] != filterCriteria.IdentifierFieldName))
                {
                    var filterList = new string[filterCriteria.UrlFilterList.Length + 1];
                    Array.Copy(filterCriteria.UrlFilterList, filterList, filterCriteria.UrlFilterList.Length);

                    filterList[filterCriteria.UrlFilterList.Length] = string.Concat(filterCriteria.IdentifierFieldName,
                        "-", filterCriteria.IdentifierFieldValue);

                    filterCriteria.UrlFilterList = filterList;
                }
        }

        private static string GetBaseUrl(string baseUrl)
        {
            return string.IsNullOrEmpty(baseUrl) ? Instance._baseUrl : baseUrl;
        }

        private static string GetChannel(string channel)
        {
            return string.IsNullOrEmpty(channel) ? Instance._channel : channel;
        }

        /// <summary>
        ///     Returns a url encoded url string for query
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="channel"></param>
        /// <param name="fieldList"></param>
        /// <param name="filterCollection"></param>
        /// <param name="sortField"></param>
        /// <param name="sortBy"></param>
        /// <param name="start"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static string CreateUrl(FilterCriteria filterCriteria)
        {
            if (!string.IsNullOrEmpty(filterCriteria.OverrideQuery))
                return
                    $"{GetBaseUrl(filterCriteria.BaseUrl)}/{GetChannel(filterCriteria.Channel)}/select?{filterCriteria.OverrideQuery}";

            var q = "*:*".UrlEncode();

            var fieldString = StringFormatters.GetFieldsString(filterCriteria.FieldList);

            var filterString = filterCriteria.FilterList != null
                ? StringFormatters.GetFiltersString(filterCriteria.FilterList)
                : filterCriteria.UrlFilterList != null
                    ? StringFormatters.GetFiltersString(filterCriteria.UrlFilterList)
                    : string.Empty;

            var sortString = StringFormatters.GetSortString(filterCriteria.SortFieldName,
                filterCriteria.SortBy);

            var facetString = filterCriteria.FacetQuery
                ? $"&facet=true&facet.field={filterCriteria.FacetFieldName}"
                : "";

            var url =
                $"{GetBaseUrl(filterCriteria.BaseUrl)}/{GetChannel(filterCriteria.Channel)}/select?start={filterCriteria.Start}&rows={filterCriteria.Rows}&q={q}{fieldString}{filterString}{sortString}{facetString}&wt=json";

            return url;
        }
    }
}
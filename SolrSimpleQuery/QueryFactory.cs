using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        Task<SolrResponse<List<TResult>>> Query<TResult>(FilterCriteria filterCriteria);

        Task<List<string>> QueryCsv(FilterCriteria filterCriteria);

        Task<string> QueryRaw(FilterCriteria filterCriteria);

        Task<SolrResponse<List<List<DynamicResult>>>> QueryDynamic(FilterCriteria filterCriteria);

        Task<SolrResponse<Dictionary<TIdentifierField, TResult>>> QueryGrouped<TIdentifierField, TResult>(
            FilterCriteria filterCriteria);

        Task<SolrResponse<Dictionary<TIdentifierField, List<DynamicResult>>>> QueryGroupedDynamic<TIdentifierField>(
            FilterCriteria filterCriteria);

        Task<dynamic> GetFieldValueByIdentifier<TIdentifierField, TResult>(
            SolrResponse<Dictionary<TIdentifierField, TResult>> resultList,
            TIdentifierField identifierFieldValue,
            string fieldName);

        Task<dynamic> GetFieldValueByIdentifier<TIdentifierField>(FilterCriteria filterCriteria, string fieldName);

        Task<TResult> GetObjectByIdentifier<TIdentifierField, TResult>(
            SolrResponse<Dictionary<TIdentifierField, TResult>> resultList,
            TIdentifierField identifierFieldValue);

        Task<TResult> GetObjectByIdentifier<TIdentifierField, TResult>(FilterCriteria filterCriteria)
            where TResult : class, new();

        Task<List<DynamicResult>> GetDynamicObjectByIdentifier<TIdentifierField>(FilterCriteria filterCriteria);
    }

    public class QueryFactory : IQueryFactory
    {
        private static QueryFactory _instance;
        private string _baseUrl = ConfigurationManager.AppSettings["SolrSimpleQueryBaseUrl"];
        private string _channel = ConfigurationManager.AppSettings["SolrSimpleQueryChannel"];

        /// <summary>
        ///     Provides a static instance of QueryFactory
        /// </summary>
        public static QueryFactory Instance => _instance ?? (_instance = new QueryFactory());

        public async Task<SolrResponse<List<TResult>>> Query<TResult>(FilterCriteria filterCriteria)
        {
            if (filterCriteria == null)
                return null;

            var result = await DoQuery<SolrResponse<List<TResult>>>(filterCriteria);

            if (filterCriteria.FacetQuery)
                result.Facet_Counts.Facet_Fields = result.Facet_Counts.GetFacetValues();

            return result;
        }

        public async Task<List<string>> QueryCsv(FilterCriteria filterCriteria)
        {
            if (filterCriteria == null)
                return null;

            return await DoQueryCsv(filterCriteria);
        }

        public async Task<string> QueryRaw(FilterCriteria filterCriteria)
        {
            if (filterCriteria == null)
                return null;

            return await DoQuery(filterCriteria);
        }

        public async Task<SolrResponse<List<List<DynamicResult>>>> QueryDynamic(FilterCriteria filterCriteria)
        {
            if (filterCriteria == null)
                return null;

            var requestResponse = new SolrResponse<List<List<DynamicResult>>>();
            var valueCollection = new List<List<DynamicResult>>();

            var result = await DoQuery<SolrResponse<List<dynamic>>>(filterCriteria);

            if (filterCriteria.FacetQuery)
                requestResponse.Facet_Counts = new FacetResponse { Facet_Fields = result.Facet_Counts.GetFacetValues() };

            requestResponse.ResponseHeader = result.ResponseHeader;
            requestResponse.Response = new Response<List<List<DynamicResult>>>
            {
                NumFound = result.Response.NumFound,
                Start = result.Response.Start,
                Docs = new List<List<DynamicResult>> { new List<DynamicResult>() }
            };

            foreach (var row in result.Response.Docs)
            {
                var resultPairs = new List<DynamicResult>();

                foreach (var prop in row)
                    try
                    {
                        if (filterCriteria.FieldsList.Any() &&
                            filterCriteria.FieldsList.All(r => r != prop.Name)) continue;

                        var properties = ((JProperty)prop).Value.ToArray();
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

        public async Task<SolrResponse<Dictionary<TIdentifierField, TResult>>> QueryGrouped<TIdentifierField, TResult>(
            FilterCriteria filterCriteria)
        {
            if (filterCriteria == null)
                return null;

            var requestResponse = new SolrResponse<Dictionary<TIdentifierField, TResult>>();
            var valueCollection = new Dictionary<TIdentifierField, TResult>();

            var result = await DoQuery<SolrResponse<List<TResult>>>(filterCriteria);

            if (filterCriteria.FacetQuery)
                requestResponse.Facet_Counts = new FacetResponse { Facet_Fields = result.Facet_Counts.GetFacetValues() };

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
                    (TIdentifierField)Convert.ChangeType(identifierFieldValue, typeof(TIdentifierField)), row);
            }

            requestResponse.Response.Docs = valueCollection;

            return requestResponse;
        }

        public async Task<SolrResponse<Dictionary<TIdentifierField, List<DynamicResult>>>> QueryGroupedDynamic<TIdentifierField>(
            FilterCriteria filterCriteria)
        {
            if (filterCriteria == null)
                return null;

            var requestResponse = new SolrResponse<Dictionary<TIdentifierField, List<DynamicResult>>>();
            var valueCollection = new Dictionary<TIdentifierField, List<DynamicResult>>();

            if (!filterCriteria.FieldsList.Contains(filterCriteria.IdentifierFieldName))
            {
                filterCriteria.FieldsList.Add(filterCriteria.IdentifierFieldName);
            }

            var result = await DoQuery<SolrResponse<List<dynamic>>>(filterCriteria);

            if (filterCriteria.FacetQuery)
                requestResponse.Facet_Counts = new FacetResponse { Facet_Fields = result.Facet_Counts.GetFacetValues() };

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
                            (TIdentifierField)Convert.ChangeType(prop.First.Value, typeof(TIdentifierField));

                    try
                    {
                        if (filterCriteria.FieldsList.Any() &&
                            filterCriteria.FieldsList.All(r => r != prop.Name)) continue;

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

        public Task<dynamic> GetFieldValueByIdentifier<TIdentifierField, TResult>(
            SolrResponse<Dictionary<TIdentifierField, TResult>> resultList,
            TIdentifierField identifierFieldValue,
            string fieldName)
        {
            return resultList.GetFieldValueByIdentifier(identifierFieldValue, fieldName);
        }

        public async Task<dynamic> GetFieldValueByIdentifier<TIdentifierField>(FilterCriteria filterCriteria, string fieldName)
        {
            if (filterCriteria == null)
                return null;

            var resultList = await QueryGroupedDynamic<TIdentifierField>(filterCriteria);
            return resultList.GetFieldValueByIdentifier(
                    (TIdentifierField)Convert.ChangeType(filterCriteria.IdentifierFieldValue,
                        typeof(TIdentifierField)),
                    fieldName)
                .Value;
        }

        public async Task<TResult> GetObjectByIdentifier<TIdentifierField, TResult>(
            SolrResponse<Dictionary<TIdentifierField, TResult>> resultList,
            TIdentifierField identifierFieldValue)
        {
            return await Task.Run(() => resultList.GetObjectByIdentifier(identifierFieldValue));
        }

        public async Task<TResult> GetObjectByIdentifier<TIdentifierField, TResult>(FilterCriteria filterCriteria)
            where TResult : class, new()
        {
            if (filterCriteria == null)
                return null;

            var resultList = await QueryGrouped<TIdentifierField, TResult>(filterCriteria);
            return resultList.GetObjectByIdentifier(
                (TIdentifierField)Convert.ChangeType(filterCriteria.IdentifierFieldValue, typeof(TIdentifierField)));
        }

        public async Task<List<DynamicResult>> GetDynamicObjectByIdentifier<TIdentifierField>(FilterCriteria filterCriteria)
        {
            if (filterCriteria == null)
                return null;

            var resultList = await QueryGroupedDynamic<TIdentifierField>(filterCriteria);
            return resultList.GetObjectByIdentifier(
                (TIdentifierField)Convert.ChangeType(filterCriteria.IdentifierFieldValue, typeof(TIdentifierField)));
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
        private static async Task<TResult> DoQuery<TResult>(FilterCriteria filterCriteria)
        {
            var resultString = await DoQuery(filterCriteria);
            return JsonConvert.DeserializeObject<TResult>(resultString);
        }

        private static async Task<string> DoQuery(FilterCriteria filterCriteria)
        {
            CheckFilterCriteriaForIdentifierValue(filterCriteria);

            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(CreateUrl(filterCriteria));

                return await result.Content.ReadAsStringAsync();
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
        private static async Task<List<string>> DoQueryCsv(FilterCriteria filterCriteria)
        {
            CheckFilterCriteriaForIdentifierValue(filterCriteria);

            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(CreateUrl(filterCriteria));
                var resultString = await result.Content.ReadAsStringAsync();
                var processedResult = resultString.Split(',').ToList();

                processedResult.Sort();

                return processedResult;
            }
        }

        private static void CheckFilterCriteriaForIdentifierValue(FilterCriteria filterCriteria)
        {
            if (filterCriteria.IdentifierFieldName == null || filterCriteria.IdentifierFieldValue == null) return;

            if (filterCriteria.FiltersList == null && filterCriteria.UrlFiltersList == null)
                filterCriteria.FiltersList = new List<IFilter>();

            if (filterCriteria.FiltersList != null && filterCriteria.FiltersList.Any())
            {
                filterCriteria.UrlFiltersList = new List<string>(); // give preference to IFilter filterList

                if (filterCriteria.FiltersList.All(r => r.FieldName != filterCriteria.IdentifierFieldName))
                {
                    filterCriteria.FiltersList.Add(
                        new SimpleFilter<string>().Create(filterCriteria.IdentifierFieldName,
                            filterCriteria.IdentifierFieldValue));
                }
            }

            if (filterCriteria.UrlFiltersList == null) return;
            {
                if (filterCriteria.UrlFiltersList.All(r => r.Split('-')[0] != filterCriteria.IdentifierFieldName))
                {
                    filterCriteria.UrlFiltersList.Add(string.Concat(filterCriteria.IdentifierFieldName,
                        "-", filterCriteria.IdentifierFieldValue));
                }
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

            var fieldString = StringFormatters.GetFieldsString(filterCriteria.FieldsList);

            var filterString = filterCriteria.FiltersList.Any()
                ? StringFormatters.GetFiltersString(filterCriteria.FiltersList)
                : filterCriteria.UrlFiltersList.Any()
                    ? StringFormatters.GetFiltersString(filterCriteria.UrlFiltersList)
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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SolrSimpleQuery.Models;

namespace SolrSimpleQuery
{
    public interface IMetaFactory
    {
        int CheckForFields(FilterCriteria filterCriteria);

        string[] GetAllAvailableFields(FilterCriteria filterCriteria);
        string[] GetAvailableFields(FilterCriteria filterCriteria);
    }

    public class MetaFactory : IMetaFactory
    {
        private static MetaFactory _instance;
        private string _baseUrl = ConfigurationManager.AppSettings["SolrSimpleQueryBaseUrl"];
        private string _channel = ConfigurationManager.AppSettings["SolrSimpleQueryChannel"];

        /// <summary>
        ///     Provides a static instance of QueryFactory
        /// </summary>
        public static MetaFactory Instance => _instance ?? (_instance = new MetaFactory());

        public int CheckForFields(FilterCriteria filterCriteria)
        {
            if (filterCriteria.FieldList == null || !filterCriteria.FieldList.Any())
                return 0;

            try
            {
                var result = QueryFactory.Instance.QueryDynamic(filterCriteria);

                return result.Response.Docs.Count(r => r.Count == filterCriteria.FieldList.Length);
            }
            catch // when the field to sort on does not exist - this the value does not exist
            {
                return 0;
            }
        }

        public string[] GetAllAvailableFields(FilterCriteria filterCriteria)
        {
            filterCriteria.OverrideQuery = "q=*%3A*&rows=0&wt=csv&facet=true";
            return QueryFactory.Instance.QueryCsv(filterCriteria);
        }

        public string[] GetAvailableFields(FilterCriteria filterCriteria)
        {
            var fields = new List<string>();

            var result = QueryFactory.Instance.Query<dynamic>(filterCriteria);

            foreach (var row in result.Response.Docs)
            foreach (var prop in row)
            {
                var fieldName = prop.Name;

                if (!string.IsNullOrEmpty(fieldName) && !fields.Contains(fieldName))
                    fields.Add(fieldName);
            }

            fields.Sort();

            return fields.ToArray();
        }

        public static void SetBaseUrl(string baseUrl)
        {
            Instance._baseUrl = baseUrl;
        }

        public static void SetChannel(string channel)
        {
            Instance._channel = channel;
        }

        private static string GetBaseUrl(string baseUrl)
        {
            return string.IsNullOrEmpty(baseUrl) ? Instance._baseUrl : baseUrl;
        }

        private static string GetChannel(string channel)
        {
            return string.IsNullOrEmpty(channel) ? Instance._channel : channel;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SolrSimpleQuery.Models;

namespace SolrSimpleQuery
{
    public interface IMetaFactory
    {
        Task<int> CheckForFields(FilterCriteria filterCriteria);

        Task<List<string>> GetAllAvailableFields(FilterCriteria filterCriteria);
        Task<List<string>> GetAvailableFields(FilterCriteria filterCriteria);
    }

    public class MetaFactory : IMetaFactory
    {
        private static MetaFactory _instance;

        /// <summary>
        ///     Provides a static instance of QueryFactory
        /// </summary>
        public static MetaFactory Instance => _instance ?? (_instance = new MetaFactory());

        public async Task<int> CheckForFields(FilterCriteria filterCriteria)
        {
            if (filterCriteria.FieldsList == null || !filterCriteria.FieldsList.Any())
                return 0;

            try
            {
                var result = await QueryFactory.Instance.QueryDynamic(filterCriteria);

                return result.Response.Docs.Count(r => r.Count == filterCriteria.FieldsList.Count);
            }
            catch // when the field to sort on does not exist - this the value does not exist
            {
                return 0;
            }
        }

        public async Task<List<string>> GetAllAvailableFields(FilterCriteria filterCriteria)
        {
            filterCriteria.OverrideQuery = "q=*%3A*&rows=0&wt=csv&facet=true";
            return await QueryFactory.Instance.QueryCsv(filterCriteria);
        }

        public async Task<List<string>> GetAvailableFields(FilterCriteria filterCriteria)
        {
            var fields = new List<string>();

            var result = await QueryFactory.Instance.Query<dynamic>(filterCriteria);

            foreach (var row in result.Response.Docs)
            foreach (var prop in row)
            {
                var fieldName = prop.Name;

                if (!string.IsNullOrEmpty(fieldName) && !fields.Contains(fieldName))
                    fields.Add(fieldName);
            }

            fields.Sort();

            return fields;
        }
    }
}
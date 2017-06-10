using System.Collections.Generic;
using System.Linq;
using SolrSimpleQuery.Filters.Interfaces;

namespace SolrSimpleQuery.Models
{
    public class FilterCriteria
    {
        private List<IFilter> _filtersList = new List<IFilter>();
        private List<string> _urlFiltersList = new List<string>();
        private List<string> _fieldsList = new List<string>();


        public string IdentifierFieldName { get; set; }
        public string IdentifierFieldValue { get; set; }

        public IFilter[] Filters
        {
            get => _filtersList.ToArray();
            set => _filtersList = value.ToList();
        }

        public List<IFilter> FiltersList
        {
            get => _filtersList;
            set => _filtersList = value;
        }

        public string[] UrlFilters
        {
            get => _urlFiltersList.ToArray();
            set => _urlFiltersList = value.ToList();
        }

        public List<string> UrlFiltersList
        {
            get => _urlFiltersList;
            set => _urlFiltersList = value;
        }

        public string[] Fields
        {
            get => _fieldsList?.ToArray() ?? new[] { IdentifierFieldName };
            set => _fieldsList = value.ToList();
        }

        public List<string> FieldsList
        {
            get => _fieldsList ?? new List<string> { IdentifierFieldName };
            set => _fieldsList = value;
        }

        public string SortFieldName { get; set; }
        public string SortBy { get; set; }
        public int Start { get; set; } = 0;
        public int Rows { get; set; } = 1;
        public string OverrideQuery { get; set; }
        public string BaseUrl { get; set; }
        public string Channel { get; set; }

        public bool FacetQuery { get; set; } = false;
        public string FacetFieldName { get; set; }
    }
}
using SolrSimpleQuery.Filters.Interfaces;

namespace SolrSimpleQuery.Models
{
    public class FilterCriteria
    {
        private string[] _fieldList;

        public string IdentifierFieldName { get; set; }
        public string IdentifierFieldValue { get; set; }
        public IFilter[] FilterList { get; set; }
        public string[] UrlFilterList { get; set; }

        public string[] FieldList
        {
            get => _fieldList ?? new[] {IdentifierFieldName};
            set => _fieldList = value;
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
using Newtonsoft.Json;
using SolrSimpleQuery.Filters.Interfaces;
using SolrSimpleQuery.Utility.Extensions;

namespace SolrSimpleQuery.Filters
{
    public class SimpleFilter<T> : IFilter
    {
        private bool _searchExtended;

        public T Value { get; private set; }

        public bool SearchExtended
        {
            get => _searchExtended &&
                   typeof(T) == typeof(string); //only extend search for strings and if explicitly set
            private set => _searchExtended = value;
        }

        public string FieldName { get; private set; }

        [JsonIgnore]
        public new string ToString => string.Concat(FieldName, UrlExt.UrlColon,
            Value.GetStringValue() + (SearchExtended ? "*" : string.Empty));

        public SimpleFilter<T> Create(string fieldName, T value, bool searchExtended = false)
        {
            FieldName = fieldName;
            Value = value;
            SearchExtended = searchExtended;

            return this;
        }

        public void SetFieldName(string fieldName)
        {
            FieldName = fieldName;
        }

        public void SetValue(T value)
        {
            Value = value;
        }

        public void SetSearchExtended(bool searchExtended)
        {
            SearchExtended = searchExtended;
        }
    }
}
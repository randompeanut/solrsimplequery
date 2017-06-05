using Newtonsoft.Json;
using SolrSimpleQuery.Filters.Interfaces;
using SolrSimpleQuery.Utility.Extensions;

namespace SolrSimpleQuery.Filters
{
    public class RangeFilter<T> : IRangeFilter<T>
    {
        public string FieldName { get; private set; }

        public T FromValue { get; private set; }

        public T ToValue { get; private set; }

        [JsonIgnore]
        public bool FromIsSpecified { get; private set; }

        [JsonIgnore]
        public bool ToIsSpecified { get; private set; }

        [JsonIgnore]
        public new string ToString =>
            string.Concat(FieldName, UrlExt.UrlColon,
                $"[{(FromIsSpecified ? FromValue.GetStringValue() : "*")} TO {(ToIsSpecified ? ToValue.GetStringValue() : "*")}]");

        public RangeFilter<T> CreateFrom(string fieldName, T fromValue)
        {
            SetFieldName(fieldName);
            SetFromValue(fromValue);

            return this;
        }

        public RangeFilter<T> CreateTo(string fieldName, T toValue)
        {
            SetFieldName(fieldName);
            SetToValue(toValue);

            return this;
        }

        public RangeFilter<T> Create(string fieldName, T fromValue, T toValue)
        {
            SetFieldName(fieldName);
            SetFromValue(fromValue);
            SetToValue(toValue);

            return this;
        }

        public void SetFieldName(string fieldName)
        {
            FieldName = fieldName;
        }

        public void SetFromValue(T value)
        {
            FromValue = value;
            FromIsSpecified = true;
        }

        public void SetToValue(T value)
        {
            ToValue = value;
            ToIsSpecified = true;
        }
    }
}
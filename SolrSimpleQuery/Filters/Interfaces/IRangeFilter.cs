using Newtonsoft.Json;

namespace SolrSimpleQuery.Filters.Interfaces
{
    public interface IRangeFilter<out T> : IFilter
    {
        T FromValue { get; }
        T ToValue { get; }

        [JsonIgnore]
        bool FromIsSpecified { get; }

        [JsonIgnore]
        bool ToIsSpecified { get; }
    }
}
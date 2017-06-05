namespace SolrSimpleQuery.Filters.Interfaces
{
    public interface IFilter
    {
        string FieldName { get; }
        string ToString { get; }
    }
}
namespace SolrSimpleQuery.Models
{
    public class DynamicResult
    {
        public DynamicResult()
        {
        }

        public DynamicResult(string fieldName, dynamic value)
        {
            Name = fieldName;
            Value = value;
        }

        public string Name { get; }
        public dynamic Value { get; }
    }
}
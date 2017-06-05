namespace SolrSimpleQuery.Models
{
    public class Response<TResult>
    {
        public long NumFound { get; set; }
        public long Start { get; set; }

        public TResult Docs { get; set; }
    }
}
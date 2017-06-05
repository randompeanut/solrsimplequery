namespace SolrSimpleQuery.Models
{
    public class SolrResponse<TResult>
    {
        public ResponseHeader ResponseHeader { get; set; }
        public Response<TResult> Response { get; set; }

        public FacetResponse Facet_Counts { get; set; }
    }
}
using System;

namespace SolrSimpleQuery.Tests
{
    public class QueryResultObject
    {
        public int GlobalId { get; set; }

        public string BronCode { get; set; }

        public string ZoekType { get; set; }

        public DateTime PublicatieDatum { get; set; }

        public bool IsKoop { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SolrSimpleQuery.Models;

namespace SolrSimpleQuery.Api.Controllers
{
    [RoutePrefix("api/meta")]
    public class MetaController : ApiController
    {
        private readonly IMetaFactory _metaFactory;

        public MetaController()
        {
        }

        public MetaController(IMetaFactory metaFactory)
        {
            _metaFactory = metaFactory;
        }

        [HttpPost]
        [Route("getallavailablefields")]
        public virtual IHttpActionResult GetAllAvailableFields(FilterCriteria filterCriteria)
        {
            var result = _metaFactory.GetAllAvailableFields(filterCriteria);

            return Ok(result);
        }

        [HttpPost]
        [Route("getavailablefields")]
        public virtual IHttpActionResult GetAvailableFields(FilterCriteria filterCriteria)
        {
            var result =
                _metaFactory.GetAvailableFields(filterCriteria);

            return Ok(result);
        }

        [HttpPost]
        [Route("checkforfields")]
        public virtual IHttpActionResult CheckForFields(FilterCriteria filterCriteria)
        {
            var result = MetaFactory.Instance.CheckForFields(filterCriteria);

            return Ok(result);
        }

        [HttpGet]
        [Route("getAllIndexerChannels")]
        public virtual IHttpActionResult GetAllIndexerChannels()
        {
            var channels = new[]
                {"fib", "funda", "geo", "landelijk", "makelaars", "presentatie-objecten", "zoekprofiel"};

            return Ok(channels);
        }

        [HttpGet]
        [Route("getAllIndexerEndPoints")]
        public virtual IHttpActionResult GetAllIndexerEndPoints()
        {
            var endPoints = new List<string>();
            var environments = new List<string>
            {
                "int",
                "projecta",
                "projectb",
                "projectc",
                "projectd",
                "projecte"
            };

            var searcherExclusions = new List<string>();

            environments.ForEach(environment =>
            {
                endPoints.Add($"http://www.{environment}.dev.funda.nl:8080/indexer");

                if (searcherExclusions.All(r => r != environment))
                    endPoints.Add($"http://www.{environment}.dev.funda.nl:8080/searcher");
            });

            return Ok(endPoints);
        }
    }
}
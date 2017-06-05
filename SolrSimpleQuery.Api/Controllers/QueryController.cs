using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using SolrSimpleQuery.Models;

namespace SolrSimpleQuery.Api.Controllers
{
    [RoutePrefix("api/query")]
    public class QueryController : ApiController
    {
        private readonly IQueryFactory _queryFactory;

        public QueryController()
        {
        }

        public QueryController(IQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        [Route("querygroupeddynamic")]
        public virtual IHttpActionResult QueryGroupedDynamic(FilterCriteria filterCriteria)
        {
            var result = _queryFactory.QueryGroupedDynamic<string>(filterCriteria);

            return Ok(result);
        }

        [Route("querydynamic")]
        public virtual IHttpActionResult QueryDynamic(FilterCriteria filterCriteria)
        {
            var result = _queryFactory.QueryDynamic(filterCriteria);

            return Ok(result);
        }

        [HttpPost]
        [Route("getfieldvaluebyidentifier/{identifierFieldValue}/{fieldName}")]
        public virtual IHttpActionResult GetFieldValueByIdentifier(dynamic resultList,
            [FromUri] string identifierFieldValue, [FromUri] string fieldName)
        {
            var deserialized =
                JsonConvert.DeserializeObject<SolrResponse<Dictionary<string, List<DynamicResult>>>>(
                    resultList.ToString());

            var result = _queryFactory.GetFieldValueByIdentifier(deserialized, identifierFieldValue, fieldName);

            return Ok(result);
        }

        [HttpPost]
        [Route("getfieldvaluebyidentifier/{fieldName}")]
        public virtual IHttpActionResult GetFieldValueByIdentifier(FilterCriteria filterCriteria,
            [FromUri] string fieldName)
        {
            var result = _queryFactory.GetFieldValueByIdentifier<string>(filterCriteria, fieldName);

            return Ok(result);
        }

        [HttpPost]
        [Route("getobjectbyidentifier")]
        public virtual IHttpActionResult GetObjectByIdentifier(FilterCriteria filterCriteria)
        {
            var result = _queryFactory.GetDynamicObjectByIdentifier<string>(filterCriteria);

            return Ok(result);
        }
    }
}
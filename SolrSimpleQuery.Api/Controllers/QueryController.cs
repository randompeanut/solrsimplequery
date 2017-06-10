using System.Collections.Generic;
using System.Threading.Tasks;
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
        public virtual async Task<IHttpActionResult> QueryGroupedDynamic(FilterCriteria filterCriteria)
        {
            var result = await _queryFactory.QueryGroupedDynamic<string>(filterCriteria);

            return Ok(result);
        }

        [Route("querydynamic")]
        public virtual async Task<IHttpActionResult> QueryDynamic(FilterCriteria filterCriteria)
        {
            var result = await _queryFactory.QueryDynamic(filterCriteria);

            return Ok(result);
        }

        [HttpPost]
        [Route("getfieldvaluebyidentifier/{identifierFieldValue}/{fieldName}")]
        public virtual async Task<IHttpActionResult> GetFieldValueByIdentifier(dynamic resultList,
            [FromUri] string identifierFieldValue, [FromUri] string fieldName)
        {
            var deserialized =
                JsonConvert.DeserializeObject<SolrResponse<Dictionary<string, List<DynamicResult>>>>(
                    resultList.ToString());

            var result = await _queryFactory.GetFieldValueByIdentifier(deserialized, identifierFieldValue, fieldName);

            return Ok(result);
        }

        [HttpPost]
        [Route("getfieldvaluebyidentifier/{fieldName}")]
        public virtual async Task <IHttpActionResult> GetFieldValueByIdentifier(FilterCriteria filterCriteria,
            [FromUri] string fieldName)
        {
            var result = await _queryFactory.GetFieldValueByIdentifier<string>(filterCriteria, fieldName);

            return Ok(result);
        }

        [HttpPost]
        [Route("getobjectbyidentifier")]
        public virtual async Task<IHttpActionResult> GetObjectByIdentifier(FilterCriteria filterCriteria)
        {
            var result = await _queryFactory.GetDynamicObjectByIdentifier<string>(filterCriteria);

            return Ok(result);
        }
    }
}
using System.Web.Http;
using System.Web.Http.Cors;
using SolrSimpleQuery.Api.App_Start;

namespace SolrSimpleQuery.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors(new EnableCorsAttribute("http://localhost:3000", "*", "*"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );

            StructuremapWebApi.Start();
        }
    }
}
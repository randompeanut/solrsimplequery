using System.Web;

namespace SolrSimpleQuery.Utility.Extensions
{
    public static class UrlExt
    {
        public static string UrlCommma => UrlEncode(",");
        public static string UrlColon => UrlEncode(":");

        public static string UrlEncode(this string value)
        {
            return HttpUtility.UrlEncode(value);
        }
    }
}
using System.Web.Http;

namespace WebcalAPI
{
    public class FilterConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new AuthorizeAttribute());
        }
    }
}
using System.Web.Http;
using WebcalAPI.Core;

namespace WebcalAPI
{
    public class FilterConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new AuthorizationAttribute());
        }
    }
}
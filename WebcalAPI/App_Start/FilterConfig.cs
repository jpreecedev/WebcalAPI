namespace WebcalAPI
{
    using System.Web.Http;

    public class FilterConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new AuthorizeAttribute());
        }
    }
}
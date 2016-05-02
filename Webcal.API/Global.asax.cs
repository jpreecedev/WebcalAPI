namespace Webcal.API
{
    using System.Data.Entity;
    using System.Web;
    using System.Web.Http;
    using Core;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.Register(GlobalConfiguration.Configuration);

            Database.SetInitializer(new Initializer());

            var formatters = GlobalConfiguration.Configuration.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
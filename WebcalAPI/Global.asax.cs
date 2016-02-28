namespace WebcalAPI
{
    using System.Data.Entity;
    using System.Web;
    using System.Web.Http;
    using Core;

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Database.SetInitializer(new Initializer());
        }
    }
}
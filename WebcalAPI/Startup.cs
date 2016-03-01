using Microsoft.Owin;
using Owin;
using WebcalAPI;

[assembly: OwinStartup(typeof (Startup))]

namespace WebcalAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
        }
    }
}
using Microsoft.Owin;
using Owin;
using Webcal.API;

[assembly: OwinStartup(typeof (Startup))]

namespace Webcal.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
        }
    }
}
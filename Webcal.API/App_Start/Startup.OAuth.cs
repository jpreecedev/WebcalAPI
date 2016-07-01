namespace Webcal.API
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Core;
    using Identity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.DataHandler.Encoder;
    using Microsoft.Owin.Security.DataProtection;
    using Microsoft.Owin.Security.Jwt;
    using Microsoft.Owin.Security.OAuth;
    using Owin;

    public class ConnectUserStore : UserStore<ConnectUser, ConnectRole, int, ConnectUserLogin, ConnectUserRole, ConnectUserClaim>
    {
        public ConnectUserStore(DbContext context)
            : base(context)
        {
        }
    }

    public class ApplicationUserManager : UserManager<ConnectUser, int>
    {
        public ApplicationUserManager(IUserStore<ConnectUser, int> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var provider = new DpapiDataProtectionProvider("WebcalAPI");

            return new ApplicationUserManager(new ConnectUserStore(context.Get<ConnectContext>()))
            {
                UserTokenProvider = new DataProtectorTokenProvider<ConnectUser, int>(provider.Create("EmailConfirmation"))
            };
        }
    }

    public partial class Startup
    {
        public void ConfigureOAuth(IAppBuilder app)
        {
            var issuer = ConfigurationManager.AppSettings["issuer"];
            var audience = ConfigurationManager.AppSettings["audience"];
            var secret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["secret"]);

            app.CreatePerOwinContext(ConnectContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] {audience},
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                {
                    new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                }
            });

            var oauthServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth2/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(90),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat(issuer)
            };

//#if DEBUG
//            oauthServerOptions.AllowInsecureHttp = true;
//#endif

            app.UseOAuthAuthorizationServer(oauthServerOptions);
        }
    }
}
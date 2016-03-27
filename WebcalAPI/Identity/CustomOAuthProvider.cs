namespace WebcalAPI.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Core;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.OAuth;

    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] {"*"});

            ConnectUser user;
            if (!AuthenticateUser(context, out user))
            {
                return Task.FromResult<object>(null);
            }

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                {"audience", context.ClientId ?? string.Empty}
            });

            var ticket = new AuthenticationTicket(SetClaimsIdentity(context, user), props);
            context.Validated(ticket);

            return Task.FromResult<object>(null);
        }
        
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                context.SetError("invalid_clientId", "client_Id is not set");
                return Task.FromResult<object>(null);
            }

            context.Validated();
            return Task.FromResult<object>(null);
        }

        private static ClaimsIdentity SetClaimsIdentity(OAuthGrantResourceOwnerCredentialsContext context, ConnectUser user)
        {
            var identity = new ClaimsIdentity("JWT");
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim("sub", context.UserName));

            var userRoles = context.OwinContext.Get<ApplicationUserManager>().GetRoles(user.Id);
            if (userRoles.Any(r => r == ConnectRoles.Admin))
            {
                userRoles = ConnectRoles.All;
            }

            foreach (var role in userRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            SetPrincipal(new ClaimsPrincipal(identity));
            return identity;
        }

        private static bool AuthenticateUser(OAuthGrantResourceOwnerCredentialsContext context, out ConnectUser user)
        {
            user = null;

            if (string.IsNullOrEmpty(context.UserName) || string.IsNullOrEmpty(context.Password))
            {
                context.SetError("invalid_grant", "The user name or password is incorrect");
                return false;
            }
            user = context.OwinContext.Get<ConnectContext>().Users.ToList().FirstOrDefault(c => string.Equals(c.CompanyKey.Replace(" ", "").Trim(), context.UserName.Replace(" ", "").Trim(), StringComparison.CurrentCultureIgnoreCase));

            if (!context.OwinContext.Get<ApplicationUserManager>().CheckPassword(user, context.Password))
            {
                context.SetError("invalid_grant", "The user name or password is incorrect");
                return false;
            }

            return true;
        }

        private static void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }
    }
}
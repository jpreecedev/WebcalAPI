using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WebcalAPI.Core
{
    public class AuthorizationAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var currentPrincipal = HttpContext.Current.User as ClaimsPrincipal;
            if (currentPrincipal != null && CheckRoles(currentPrincipal))
            {
                return true;
            }

            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            return false;
        }

        private bool CheckRoles(IPrincipal principal)
        {
            if (Roles == null || !Roles.Any())
            {
                return true;
            }

            var result = Roles.Split(',').Where(s => !string.IsNullOrWhiteSpace(s.Trim())).ToList();
            return result.Count == 0 || result.Any(principal.IsInRole);
        }
    }
}
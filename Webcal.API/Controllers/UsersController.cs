namespace Webcal.API.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Web.Http;

    [RoutePrefix("api/user")]
    public class UserController : BaseApiController
    {
        [HttpGet]
        [Route("roles")]
        public IHttpActionResult GetRoles()
        {
            var roles = string.Join(",", ((ClaimsIdentity) User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));
            return Ok(roles);
        }
    }
}
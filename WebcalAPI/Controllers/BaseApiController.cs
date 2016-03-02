namespace WebcalAPI.Controllers
{
    using System.Net.Http;
    using System.Web.Http;
    using Microsoft.AspNet.Identity.Owin;

    public class BaseApiController : ApiController
    {
        private ApplicationUserManager _userManager;

        protected ApplicationUserManager UserManager
        {
            get { return _userManager ?? (_userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>()); }
        }
    }
}
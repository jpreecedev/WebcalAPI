namespace Webcal.API.Controllers
{
    using System;
    using Connect.Shared.Models;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core;
    using Model;

    [AllowAnonymous]
    [RoutePrefix("api/mobile")]
    public class MobileController : BaseApiController
    {
        [HttpPost]
        [Route("authenticate")]
        public async Task<IHttpActionResult> Authenticate([FromBody]MobileAppRequestViewModel<object> message)
        {
            if (message == null || await IsUserAuthorized(message) == null)
            {
                return Unauthorized();
            }

            return Ok();
        }

        [HttpPost]
        [Route("qcreport")]
        public async Task<IHttpActionResult> PostQcReport([FromBody]MobileAppRequestViewModel<QCReport> message)
        {
            var mobileApplicationUser = await IsUserAuthorized(message);
            if (mobileApplicationUser == null)
            {
                return Unauthorized();
            }

            if (message.Username == "LeeTest")
            {
                return Ok();
            }

            message.Data.User = mobileApplicationUser;
            message.Data.MobileDocumentType = ReportType.QCReport;
            message.Data.Created = DateTime.Now;

            using (var context = new ConnectContext())
            {
                context.QCReports.Add(message.Data);
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPost]
        [Route("qcreport6month")]
        public async Task<IHttpActionResult> PostQcReport6Month([FromBody]MobileAppRequestViewModel<QCReport6Month> message)
        {
            var mobileApplicationUser = await IsUserAuthorized(message);
            if (mobileApplicationUser == null)
            {
                return Unauthorized();
            }

            if (message.Username == "LeeTest")
            {
                return Ok();
            }

            message.Data.User = mobileApplicationUser;
            message.Data.MobileDocumentType = ReportType.QCReport6Month;
            message.Data.Created = DateTime.Now;

            using (var context = new ConnectContext())
            {
                context.QCReports6Month.Add(message.Data);
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        private static async Task<MobileApplicationUser> IsUserAuthorized<T>(MobileAppRequestViewModel<T> message)
        {
            if (string.IsNullOrEmpty(message?.Username) || string.IsNullOrEmpty(message.Thumbprint))
            {
                return null;
            }

            using (var context = new ConnectContext())
            {
                var user = await context.MobileApplicationUsers.FirstOrDefaultAsync(u => u.Username == message.Username && u.Thumbprint == message.Thumbprint);
                if (user != null)
                {
                    return user.IsAuthorized ? user : null;
                }

                user = context.MobileApplicationUsers.Add(new MobileApplicationUser
                {
                    Username = message.Username,
                    Thumbprint = message.Thumbprint,
                    IsAuthorized = true
                });
                await context.SaveChangesAsync();
                return user;
            }
        }
    }
}
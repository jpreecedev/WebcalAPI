namespace Webcal.API.Controllers
{
    using Connect.Shared.Models;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core;
    
    [AllowAnonymous]
    [RoutePrefix("api/mobile")]
    public class MobileApplicationController : BaseApiController
    {
        [HttpPost]
        [Route("authenticate")]
        public async Task<IHttpActionResult> Post(string username, string thumbprint)
        {
            if (await IsUserAuthorized(username, thumbprint) == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [Route("qcreport")]
        public async Task<IHttpActionResult> PostQCReport(string username, string thumbprint, QCReport data)
        {
            var mobileApplicationUser = await IsUserAuthorized(username, thumbprint);
            if (mobileApplicationUser == null)
            {
                return NotFound();
            }

            data.User = mobileApplicationUser;
            data.MobileDocumentType = ReportType.QCReport;

            using (var context = new ConnectContext())
            {
                context.QCReports.Add(data);
                return Ok();
            }
        }

        [HttpPost]
        [Route("qcreport6month")]
        public async Task<IHttpActionResult> PostQCReport6Month(string username, string thumbprint, QCReport6Month data)
        {
            var mobileApplicationUser = await IsUserAuthorized(username, thumbprint);
            if (mobileApplicationUser == null)
            {
                return NotFound();
            }

            data.User = mobileApplicationUser;
            data.MobileDocumentType = ReportType.QCReport6Month;

            using (var context = new ConnectContext())
            {
                context.QCReports6Month.Add(data);
                return Ok();
            }
        }

        private async Task<MobileApplicationUser> IsUserAuthorized(string username, string thumbprint)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(thumbprint))
            {
                return null;
            }

            using (var context = new ConnectContext())
            {
                var user = await context.MobileApplicationUsers.FirstOrDefaultAsync(u => u.IsAuthorized && u.Username == username && u.Thumbprint == thumbprint);
                if (user != null)
                {
                    return user; 
                }
            }
            return null;
        }
    }
}
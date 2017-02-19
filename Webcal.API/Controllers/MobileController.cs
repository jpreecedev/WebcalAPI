namespace Webcal.API.Controllers
{
    using System;
    using Connect.Shared.Models;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
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
        public async Task<IHttpActionResult> PostQcReport()
        {
            var message = HttpContext.Current.Request.Form.MapToQcReport();
            var mobileApplicationUser = await IsUserAuthorized(message);

            if (mobileApplicationUser == null)
            {
                return Unauthorized();
            }
            if (message.Username == "LeeTest")
            {
                return Ok();
            }
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            message.Data.User = mobileApplicationUser;
            message.Data.MobileDocumentType = ReportType.QCReport;
            message.Data.Created = DateTime.Now;

            QCReport entity;

            using (var context = new ConnectContext())
            {
                entity = context.QCReports.Add(message.Data);
                await context.SaveChangesAsync();
            }
            
            var serializedData = new SerializedData
            {
                DocumentId = entity.Id,
                Data = await ExtractSerializedData(),
                DocumentType = typeof(QCReport).FullName
            };

            using (var context = new ConnectContext())
            {
                context.Set<SerializedData>().Add(serializedData);
                await context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        [Route("qcreport6month")]
        public async Task<IHttpActionResult> PostQcReport6Month()
        {
            var message = HttpContext.Current.Request.Form.MapToQcReport6Month();

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

            QCReport6Month entity;

            using (var context = new ConnectContext())
            {
                entity = context.QCReports6Month.Add(message.Data);
                await context.SaveChangesAsync();
            }

            var serializedData = new SerializedData
            {
                DocumentId = entity.Id,
                Data = await ExtractSerializedData(),
                DocumentType = typeof(QCReport6Month).FullName
            };

            using (var context = new ConnectContext())
            {
                context.Set<SerializedData>().Add(serializedData);
                await context.SaveChangesAsync();
            }

            return Ok();
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

        private async Task<byte[]> ExtractSerializedData()
        {
            var root = HttpContext.Current.Server.MapPath("~/App_Data/uploads");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                var file = provider.FileData.FirstOrDefault();
                if (file != null)
                {
                    return File.ReadAllBytes(file.LocalFileName);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
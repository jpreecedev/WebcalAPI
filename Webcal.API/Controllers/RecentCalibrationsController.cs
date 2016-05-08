namespace Webcal.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Core;
    using Helpers;
    using Model;

    [RoutePrefix("api/recentcalibrations")]
    public class RecentCalibrationsController : BaseApiController
    {
        [HttpGet]
        [Route("{from:datetime}/{to:datetime}/{filter?}")]
        public IHttpActionResult Get(DateTime from, DateTime to, string filter = null)
        {
            using (var context = new ConnectContext())
            {
                return Ok(context.GetAllDocuments(ConnectUser)
                    .Where(c => c.Created.Date >= from.Date && c.Created.Date <= to.Date)
                    .Where(c => string.IsNullOrEmpty(filter) || (!string.IsNullOrEmpty(c.DepotName) && c.DepotName.ToUpper().Contains(filter.ToUpper())))
                    .Select(c => new RecentCalibrationsViewModel(c))
                    .OrderByDescending(c => c.Created));
            }
        }

        [HttpGet]
        [Route("{userId}/{from}")]
        public IHttpActionResult Get(int userId, DateTime from)
        {
            if (userId == -1 && !User.IsInRole(ConnectRoles.Admin))
            {
                return Unauthorized();
            }

            using (var context = new ConnectContext())
            {
                try
                {
                    return Ok(context.RecentCalibrations(ConnectUser, userId, from));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("email")]
        public IHttpActionResult Email([FromBody] CalibrationEmailViewModel calibrationData)
        {
            var body = EmailHelper.GetCalibrationDataTable(calibrationData.Calibrations);
            SendEmail(calibrationData.Recipient, "Your Recent Calibrations Data", body);
            return Ok();
        }

        [HttpPost]
        [Route("emailcertificate")]
        public async Task<IHttpActionResult> EmailCertificate([FromBody] CalibrationCertificateEmailViewModel calibrationData)
        {
            Document document;
            using (var context = new ConnectContext())
            {
                document = (Document)await context.GetDocumentAsync(calibrationData.DocumentType, calibrationData.DocumentId);
            }

            if (document != null && document.SerializedData != null)
            {
                try
                {
                    SendCertificate(calibrationData.Recipient, "Your Certificate From WebcalConnect.com", document);
                    return Ok();
                }
                catch (Exception)
                {
                    return InternalServerError();
                }
            }

            return NotFound();
        }

        protected void SendCertificate(string to, string subject, Document document)
        {
            var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Replace("-", "") + ".pdf");
            using (var fileStream = File.OpenWrite(filePath))
            {
                fileStream.Write(document.SerializedData, 0, document.SerializedData.Length);
            }

            SendEmail(to, subject, "Your certificate is attached to this email. Thanks for using <a href=\"https://www.webcalconnect.com/connect\">WebcalConnect.com</a>.", new List<Attachment>
            {
                new Attachment(filePath, new ContentType("application/pdf"))
            });
        }
    }
}
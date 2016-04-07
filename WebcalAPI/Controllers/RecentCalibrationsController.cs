namespace WebcalAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Connect.Shared.Models;
    using Core;
    using Helpers;
    using Models;

    [RoutePrefix("api/recentcalibrations")]
    public class RecentCalibrationsController : BaseApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            using (var context = new ConnectContext())
            {
                return Ok(context.GetAllDocuments(ConnectUser).Select(c => new RecentCalibrationsViewModel(c)));
            }
        }

        [HttpPost]
        [Route("email")]
        public IHttpActionResult Email([FromBody] CalibrationEmailViewModel calibrationData)
        {
            var body = EmailBuilder.GetCalibrationDataTable(calibrationData.Calibrations);
            SendEmail(calibrationData.Recipient, "Recent Calibrations", body);
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

            SendEmail(to, subject, "Your calibration certificate is attached to this email.", new List<Attachment>
            {
                new Attachment(filePath, new ContentType("application/pdf"))
            });
        }
    }
}
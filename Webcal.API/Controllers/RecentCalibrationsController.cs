using System.Data.Entity;

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
        [Route("{dateFrom:datetime}/{to:datetime}/{filter?}")]
        public IHttpActionResult Get(DateTime dateFrom, DateTime to, string filter = null)
        {
            using (var context = new ConnectContext())
            {
                var documents = from document in context.GetAllDocuments(ConnectUser)
                                join contact in context.CustomerContacts on document.CustomerContact equals contact.Name into customerContact
                                from subContact in customerContact.DefaultIfEmpty().Where(x => x?.Name != null).Where(x => x.UserId == document.UserId).DistinctBy(x => x.Name)
                                where document.Created.Date >= dateFrom.Date && document.Created.Date <= to.Date
                                where string.IsNullOrEmpty(filter) || (!string.IsNullOrEmpty(document.DepotName) && document.DepotName.ToUpper().Contains(filter.ToUpper()))
                                orderby document.Created descending
                                select new RecentCalibrationsViewModel(document, subContact);

                return Ok(documents.ToList());
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
                var fullDocumentType = calibrationData.DocumentType.GetAttributeOfType<FullDocumentTypeAttribute>().Type;
                var documentSerializedData = await context.SerializedData.FirstOrDefaultAsync(s => s.DocumentId == document.Id && s.DocumentType == fullDocumentType);
                if (documentSerializedData != null)
                {
                    document.SerializedData = documentSerializedData.Data;
                }
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
namespace WebcalAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Net.Mail;
    using System.Web.Http;
    using Connect.Shared.Models;
    using Core;
    using Helpers;
    using Microsoft.AspNet.Identity.Owin;
    using Models;

    public class BaseApiController : ApiController
    {
        private ApplicationUserManager _userManager;

        protected ApplicationUserManager UserManager
        {
            get { return _userManager ?? (_userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>()); }
        }

        protected ConnectUser ConnectUser
        {
            get { return User.GetConnectUser(); }
        }

        protected HttpResponseMessage Pdf(byte[] data, string fileName)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(Convert.ToBase64String(data))
            };
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName,
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            return result;
        }

        protected IHttpActionResult PagedResponse<T>(int pageSize, int pageIndex, IEnumerable<T> data)
        {
            return Ok(new PagedResponse<T>(data, pageIndex, pageSize));
        }

        protected static void SendEmail(string recipient, string subject, string body, List<Attachment> attachments = null)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.To.Add(recipient);
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = EmailHelper.GetBasicTemplate(subject, body);
                mailMessage.From = new MailAddress("webcal@tachoworkshop.co.uk");

                if (attachments != null)
                {
                    attachments.ForEach(c => mailMessage.Attachments.Add(c));
                }

                using (var smtp = new SmtpClient())
                {
                    smtp.Credentials = new NetworkCredential("admin@webcalconnect.com", "No05K8lGgB");
                    smtp.Host = "mail.webcalconnect.com";
                    smtp.Send(mailMessage);
                }
            }
        }
    }
}
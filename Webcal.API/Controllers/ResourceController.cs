namespace Webcal.API.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Core;
    using Microsoft.AspNet.Identity;

    [RoutePrefix("api/resource")]
    public class ResourceController : BaseApiController
    {
        [HttpGet, Authorize(Roles = "Administrator")]
        [Route("exceptionImage/{id}")]
        public async Task<HttpResponseMessage> DownloadExceptionImage(int id)
        {
            using (var context = new ConnectContext())
            {
                var exception = await context.DetailedExceptions.FirstOrDefaultAsync(c => c.Id == id);
                if (exception != null)
                {
                    return Image(exception.RawImage.Decompress(), "ExceptionScreenshot.jpg");
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("certificate/{itemId}/{itemKey}")]
        public async Task<HttpResponseMessage> DownloadCertificate(int itemId, string itemKey)
        {
            if (!string.IsNullOrEmpty(itemKey) && itemKey == FilterDocumentType.AnalogueTachograph.ToString())
            {
                itemKey = FilterDocumentType.Tachograph.ToString();
            }

            DocumentType documentType;
            if (Enum.TryParse(itemKey, out documentType))
            {
                BaseModel model;
                using (var context = new ConnectContext())
                {
                    model = await context.GetDocumentAsync(documentType, itemId);
                }

                var userId = -1;
                byte[] serializedData = null;

                var document = model as Document;
                if (document != null)
                {
                    userId = document.UserId;
                    serializedData = document.SerializedData;
                }

                var report = model as BaseReport;
                if (report != null && report.ConnectUserId != null)
                {
                    userId = report.ConnectUserId.Value;
                    serializedData = report.SerializedData;
                }

                if (userId != -1 && serializedData != null && (User.IsInRole(ConnectRoles.Admin) || User.Identity.GetUserId<int>() == userId))
                {
                    return Pdf(serializedData, $"{itemKey}.pdf");
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("directupload/{itemId}/")]
        public async Task<HttpResponseMessage> DirectUpload(int itemId)
        {
            DirectUploadDocument document;
            using (var context = new ConnectContext())
            {
                document = await context.GetDirectUploadDocumentAsync(ConnectUser, itemId);
            }

            var userId = -1;
            byte[] serializedData = null;
            
            if (document != null)
            {
                userId = document.UserId;
                serializedData = document.SerializedData;
            }
            
            if (userId != -1 && serializedData != null && (User.IsInRole(ConnectRoles.Admin) || User.Identity.GetUserId<int>() == userId))
            {
                return Pdf(serializedData, $"{document.FileName}");
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}
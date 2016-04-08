namespace WebcalAPI.Controllers
{
    using System;
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
    }
}
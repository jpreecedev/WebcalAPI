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
            DocumentType documentType;
            if (Enum.TryParse(itemKey, out documentType))
            {
                Document document;
                using (var context = new ConnectContext())
                {
                    document = (Document)await context.GetDocumentAsync(documentType, itemId);
                }

                if (User.IsInRole(ConnectRoles.Admin) || User.Identity.GetUserId<int>() == document.UserId)
                {
                    if (document != null && document.SerializedData != null)
                    {
                        return Pdf(document.SerializedData, $"{itemKey}.pdf");
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}
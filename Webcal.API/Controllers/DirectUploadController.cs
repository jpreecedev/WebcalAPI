namespace Webcal.API.Controllers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using Connect.Shared.Models;
    using Core;
    using Microsoft.AspNet.Identity;
    using Model;
    using Shared;

    [RoutePrefix("api/directupload")]
    public class DirectUploadController : BaseApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            using (var context = new ConnectContext())
            {
                var documents = await context.GetDirectUploadDocumentsAsync(ConnectUser);

                return Ok(documents.Select(c => new DirectUploadViewModel
                {
                    DocumentId = c.Id,
                    FileName = c.FileName,
                    Uploaded = DateTime.Now
                }));
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = HttpContext.Current.Server.MapPath("~/App_Data/uploads");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (var file in provider.FileData)
                {
                    var directFileUpload = new DirectUploadDocument
                    {
                        FileName = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty),
                        SerializedData = File.ReadAllBytes(file.LocalFileName),
                        Uploaded = DateTime.Now,
                        UserId = User.Identity.GetUserId<int>()
                    };

                    using (var context = new ConnectContext())
                    {
                        context.DirectUploadDocuments.Add(directFileUpload);
                        await context.SaveChangesAsync();
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
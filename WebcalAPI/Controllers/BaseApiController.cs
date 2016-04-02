namespace WebcalAPI.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;
    using Connect.Shared.Models;
    using Core;
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
    }
}
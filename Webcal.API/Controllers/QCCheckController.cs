namespace Webcal.API.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using Connect.Shared.Models;
    using Core;
    using Webcal.Model.ViewModels;

    [RoutePrefix("api/qccheck")]
    public class QCCheckController : BaseApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            using (var context = new ConnectContext())
            {
                return Ok(context.GetReports<QCReport>(ConnectUser).Select(c => new QCCheckViewModel(c)));
            }
        }
    }
}
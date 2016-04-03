namespace WebcalAPI.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using Connect.Shared.Models;
    using Core;
    using Models;

    [RoutePrefix("api/centrecheck")]
    public class CentreCheckController : BaseApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            using (var context = new ConnectContext())
            {
                return Ok(context.GetReports<QCReport6Month>(ConnectUser).Select(c => new CentreCheckViewModel(c)));
            }
        }
    }
}
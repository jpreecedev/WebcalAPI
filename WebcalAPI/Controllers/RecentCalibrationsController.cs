namespace WebcalAPI.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core;
    using Models;

    [RoutePrefix("api/recentcalibrations")]
    public class RecentCalibrationsController : BaseApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            using (var context = new ConnectContext())
            {
                return Ok(context.GetAllDocuments(ConnectUser).Select(c => new RecentCalibrationsViewModel(c)));
            }
        }
    }
}
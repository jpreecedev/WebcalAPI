namespace WebcalAPI.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using Core;
    using Models;

    [RoutePrefix("api/calibrationsdue")]
    public class CalibrationsDueController : BaseApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var from = DateTime.Now.StartOfMonth();
            var to = DateTime.Now.EndOfNextMonth();

            using (var context = new ConnectContext())
            {
                return Ok(context.GetAllDocuments(ConnectUser, from, to).Select(c => new CalibrationsDueViewModel(c)));
            }
        }
    }
}
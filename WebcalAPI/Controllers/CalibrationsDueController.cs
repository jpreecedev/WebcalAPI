namespace WebcalAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Connect.Shared;
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

        [HttpGet]
        [Route("{userId}/{from}/{to}")]
        public IHttpActionResult Get(int userId, DateTime from, DateTime to)
        {
            if (userId == -1 && !User.IsInRole(ConnectRoles.Admin))
            {
                return Unauthorized();
            }

            using (var context = new ConnectContext())
            {
                var data = context.CalibrationsDue(ConnectUser, userId, from, to);
                return Ok(data);
            }
        }
    }
}
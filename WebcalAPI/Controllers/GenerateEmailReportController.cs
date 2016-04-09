namespace WebcalAPI.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Connect.Shared;
    using Core;
    using Helpers;
    using Models;

    [RoutePrefix("api/generateemailreport")]
    public class GenerateEmailReportController : BaseApiController
    {
        public async Task<IHttpActionResult> Get()
        {
            using (var context = new ConnectContext())
            {
                var model = new GenerateEmailReportViewModel
                {
                    From = DateTime.Now.StartOfLastMonth(),
                    To = DateTime.Now.EndOfNextMonth()
                };

                var clients = await context.Users.Where(c => c.CompanyKey == ConnectUser.CompanyKey || ConnectUser.CompanyKey == ConnectConstants.ConnectAdministratonCompany)
                    .OrderBy(c => c.CompanyKey)
                    .ToListAsync();

                model.Clients = clients.Select(c => new ClientNameViewModel(c)).ToList();
                model.Clients.RemoveAll(c => c.Name == ConnectConstants.ConnectAdministratonCompany);

                if (User.IsInRole(ConnectRoles.Admin))
                {
                    model.Clients.Insert(0, new ClientNameViewModel(-1, Constants.All));
                }

                return Ok(model);
            }
        }

        public IHttpActionResult Post([FromBody]EmailReportViewModel data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.UserId == -1 && !User.IsInRole(ConnectRoles.Admin))
            {
                return Unauthorized();
            }

            using (var context = new ConnectContext())
            {
                switch (data.ReportType)
                {
                    case ReportType.RecentCalibrations:
                        var recentCalibrations = context.RecentCalibrations(ConnectUser, data.UserId, data.From);
                        SendEmail(data.Recipient, "Your Recent Calibrations Report", EmailHelper.GetCalibrationDataTable(recentCalibrations));
                        break;

                    case ReportType.CalibrationsDue:
                        var calibrationsDue = context.CalibrationsDue(ConnectUser, data.UserId, data.From, data.To.GetValueOrDefault());
                        SendEmail(data.Recipient, "Your Calibrations Due Report", EmailHelper.GetCalibrationDataTable(calibrationsDue));
                        break;
                }
            }

            return Ok();
        }
    }
}
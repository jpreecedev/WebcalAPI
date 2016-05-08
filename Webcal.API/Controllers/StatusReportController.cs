namespace Webcal.API.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using Connect.Shared;
    using Core;
    using Helpers;
    using Microsoft.AspNet.Identity;
    using Model;

    [RoutePrefix("api/statusreport")]
    public class StatusReportController : BaseApiController
    {
        [HttpGet]
        [Route("{userId:int}")]
        public IHttpActionResult Get(int userId)
        {
            using (var context = new ConnectContext())
            {
                if (userId != User.Identity.GetUserId<int>() && !User.IsInRole(ConnectRoles.Admin))
                {
                    return NotFound();
                }

                var last12Months = StatusReportHelper.GetLast12Months();
                var from = last12Months.First();
                var to = last12Months.Last().AddMonths(1).AddDays(-1);

                var statusReportData = new StatusReportData
                {
                    Technicians = context.Technicians.Where(c => c.UserId == userId).ToList(),
                    WorkshopSettings = context.WorkshopSettings.Where(c => c.UserId == userId)
                        .OrderByDescending(c => c.Created)
                        .FirstOrDefault(),
                    Last12Months = last12Months,
                    Documents = context.TachographDocuments.Where(c => c.Created >= from && c.Created <= to)
                        .Select(c => new ReportDocumentViewModel
                        {
                            Technician = c.Technician,
                            Created = c.Created
                        }).ToList()
                };

                return Ok(StatusReportHelper.GenerateStatusReport(statusReportData));
            }
        }
    }


}
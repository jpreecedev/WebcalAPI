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
        [Route("{userId?}")]
        public IHttpActionResult Get(int? userId)
        {
            var suppliedUserId = userId == null ? User.Identity.GetUserId<int>() : userId.GetValueOrDefault();
            var isAdministrator = User.IsInRole(ConnectRoles.Admin);

            using (var context = new ConnectContext())
            {
                if (suppliedUserId != User.Identity.GetUserId<int>() && !isAdministrator)
                {
                    return NotFound();
                }

                var last12Months = StatusReportHelper.GetLast12Months();
                var from = last12Months.First();
                var to = last12Months.Last().AddMonths(1).AddDays(-1);
                var technicians = context.Technicians.Where(c => c.UserId == suppliedUserId).ToList();
                var users = isAdministrator ? context.Users.Where(u => u.Deleted == null)
                    .OrderBy(c => c.CompanyKey)
                    .Select(c => new StatusReportUserViewModel {Name = c.CompanyKey, Id = c.Id})
                    .ToList() : null;

                if (technicians.Count > 0)
                {
                    var statusReportData = new StatusReportData
                    {
                        Technicians = technicians,
                        WorkshopSettings = context.WorkshopSettings.Where(c => c.UserId == suppliedUserId)
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
                    return Ok(StatusReportHelper.GenerateStatusReport(statusReportData, users));
                }

                return Ok(new StatusReportViewModel(null, null, users)
                {
                    Users = users
                });
            }
        }
    }


}
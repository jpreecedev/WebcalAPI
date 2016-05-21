namespace Webcal.API.Core
{
    using System;
    using Connect.Shared;
    using Model;

    public static class TechnicianExtensions
    {
        public static ReportItemStatus HalfYearStatus(this TechnicianViewModel technician)
        {
            if (technician == null || technician.DateOfLastCheck == null)
            {
                return ReportItemStatus.Unknown;
            }

            var lastCheck = technician.DateOfLastCheck.GetValueOrDefault();
            var nextCheckDue = lastCheck.AddMonths(6).AddDays(-7).Date;
            var expiration = lastCheck.AddMonths(6).Date;
            var now = DateTime.Now.Date;

            if (now >= nextCheckDue && now <= expiration)
            {
                return ReportItemStatus.CheckDue;
            }
            if (now >= lastCheck && now < nextCheckDue)
            {
                return ReportItemStatus.Ok;
            }
            return ReportItemStatus.Expired;
        }

        public static ReportItemStatus ThreeYearStatus(this TechnicianViewModel technician)
        {
            if (technician == null)
            {
                return ReportItemStatus.Unknown;
            }

            var lastCheck = technician.DateOfLast3YearCheck.GetValueOrDefault().Date;
            var now = DateTime.Now.Date;
            if (lastCheck > now)
            {
                return ReportItemStatus.Unknown;
            }

            var expiration = lastCheck.Date.AddYears(3).Date;
            var checkDue = expiration.AddMonths(-1).Date;

            if (now >= checkDue && now <= expiration)
            {
                return ReportItemStatus.CheckDue;
            }
            if (now < checkDue)
            {
                return ReportItemStatus.Ok;
            }
            return ReportItemStatus.Expired;
        }
    }
}
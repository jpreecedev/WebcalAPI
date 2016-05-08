namespace Webcal.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Connect.Shared;
    using Connect.Shared.Models;

    public class StatusReportViewModel
    {
        public StatusReportViewModel(List<Technician> technicians, WorkshopSettings workshopSettings)
        {
            if (technicians == null || technicians.Count == 0 || workshopSettings == null)
            {
                return;
            }

            TachoCentreLastCheck = workshopSettings.CentreQuarterlyCheckDate;
            GV212LastCheck = workshopSettings.MonthlyGV212Date;
            ColorStart = ColorStop = GetColorForScore(technicians);
            Score = CalculateScore(technicians);
        }

        private DateTime? TachoCentreLastCheck { get; }

        public DateTime? GV212LastCheck { get; set; }

        public int Score { get; set; }

        public List<StatusReportTechnician> Performance { get; set; }

        public string ColorStart { get; set; }

        public string ColorStop { get; set; }

        public List<string> LineChartLabels { get; set; }

        public List<int> LineChartData { get; set; }

        private ReportItemStatus GetGV212Status()
        {
            if (GV212LastCheck == null)
            {
                return ReportItemStatus.Unknown;
            }

            var lastCheck = GV212LastCheck.GetValueOrDefault().Date;
            var now = DateTime.Now.Date;

            if (lastCheck > now)
            {
                return ReportItemStatus.Unknown;
            }

            if (lastCheck.Date.Month == now.Month && lastCheck.Date.Year == now.Year)
            {
                return ReportItemStatus.Ok;
            }

            return ReportItemStatus.Expired;
        }

        private ReportItemStatus GetTachoCentreQuarterlyStatus()
        {
            if (TachoCentreLastCheck == null)
            {
                return ReportItemStatus.Unknown;
            }

            var lastCheck = TachoCentreLastCheck.GetValueOrDefault().Date;
            var now = DateTime.Now.Date;

            if (lastCheck > now)
            {
                return ReportItemStatus.Unknown;
            }

            var expiration = lastCheck.AddMonths(3).Date;
            var checkDue = expiration.AddDays(-7).Date;

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

        private string GetColorForScore(List<Technician> technicians)
        {
            var score = CalculateScore(technicians);
            if (score <= 50)
            {
                return "#E40213";
            }
            if (score > 50 && score <= 75)
            {
                return "#FF8C00";
            }
            if (score > 75)
            {
                return "#006400";
            }
            return "#E40213";
        }

        private int CalculateScore(IReadOnlyCollection<Technician> technicians)
        {
            var centreQuarterlyStatus = GetInflatedScore(GetTachoCentreQuarterlyStatus());
            var gv212Status = GetInflatedScore(GetGV212Status());
            
            var technicianMaxScore = technicians.Count * 4;
            var maxScore = 10 + technicianMaxScore;
            var actualScore = centreQuarterlyStatus + gv212Status + technicianMaxScore;
            var scorePercentage = (100 * maxScore) / actualScore;

            if (technicians.Any(c => ThreeYearStatus(c) == ReportItemStatus.Expired || ThreeYearStatus(c) == ReportItemStatus.Unknown))
            {
                if (scorePercentage > 75)
                {
                    return 75;
                }
            }

            return scorePercentage;
        }

        private static int GetInflatedScore(ReportItemStatus status)
        {
            switch (status)
            {
                case ReportItemStatus.Ok:
                    return 5;
                case ReportItemStatus.CheckDue:
                    return 2;
                case ReportItemStatus.Expired:
                case ReportItemStatus.Unknown:
                    return 0;
            }
            return 0;
        }
        
        private static ReportItemStatus ThreeYearStatus(Technician technician)
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
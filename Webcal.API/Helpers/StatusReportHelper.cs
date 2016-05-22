namespace Webcal.API.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Connect.Shared;
    using Core;
    using Model;
    using Properties;

    public static class StatusReportHelper
    {
        private static readonly Random _random = new Random();

        public static StatusReportViewModel GenerateStatusReport(StatusReportData data, List<StatusReportUserViewModel> users)
        {
            var result = new StatusReportViewModel(data.Technicians, data.WorkshopSettings, users)
            {
                Performance = new List<StatusReportTechnician>()
            };

            GetTechniciansPerformanceData(data, result);

            result.LineChartLabels = data.Last12Months.Select(c => c.ToString("MMM yy")).ToList();

            string tachoCentreQuarterlyStatusText;
            result.TachoCentreQuarterlyStatusTextColor = "#" + GetStatus(result.TachoCentreQuarterlyStatus, out tachoCentreQuarterlyStatusText);
            result.TachoCentreQuarterlyStatusText = tachoCentreQuarterlyStatusText;

            string gv212StatusText;
            result.GV212StatusTextColor = "#" + GetStatus(result.GV212Status, out gv212StatusText);
            result.GV212StatusText = gv212StatusText;

            return result;
        }

        public static List<DateTime> GetLast12Months()
        {
            var result = new List<DateTime>();
            var now = DateTime.Parse("01/" + DateTime.Now.Month + "/" + DateTime.Now.Year);

            for (var i = 11; i > 0; i--)
            {
                result.Add(now.AddMonths(i * -1));
            }

            result.Add(now);
            return result;
        }

        private static void GetTechniciansPerformanceData(StatusReportData data, StatusReportViewModel result)
        {
            var technicianViewModels = data.Technicians.Select(c => new TechnicianViewModel(data.Documents, data.Last12Months, c)).ToList();
            for (var index = 0; index < technicianViewModels.Count; index++)
            {
                var technician = technicianViewModels[index];

                string halfYearStatusText;
                var halfYearColor = GetStatus(technician.HalfYearStatus(), out halfYearStatusText);

                string threeYearStatusText;
                var threeYearColor = GetStatus(technician.ThreeYearStatus(), out threeYearStatusText);

                string statusTextColor;
                var statusText = GetStatusText(halfYearStatusText, threeYearStatusText, out statusTextColor);

                result.Performance.Add(new StatusReportTechnician
                {
                    Value = technician.JobsDoneInLast12Months,
                    Color = GetColor(index),
                    Label = technician.Technician.Name.ToTitleCase(),
                    DateOfNextCheck = technician.DateOfLastCheck == null ? (DateTime?)null : technician.DateOfLastCheck.GetValueOrDefault().AddMonths(6),
                    TrainingDateExpiry = technician.DateOfLast3YearCheck == null ? (DateTime?)null : technician.DateOfLast3YearCheck.GetValueOrDefault().AddYears(3),
                    StatusColor = "#" + halfYearColor,
                    ThreeYearColor = "#" + threeYearColor,
                    Status = statusText
                });
            }

            result.LineChartData = new List<int>();
            foreach (var month in data.Last12Months)
            {
                var count = technicianViewModels.Sum(technicianViewModel => technicianViewModel.JobsMonthByMonth.First(d => d.Key == month).Value);
                result.LineChartData.Add(count);
            }
        }

        private static string GetColor(int index)
        {
            var basicColors = new[] { "#09355C", "#CBCBCB", "#E40213" };
            var basicColor = basicColors.ElementAtOrDefault(index);
            if (basicColor == null)
            {
                var color = $"#{_random.Next(0x1000000):X6}";
                return color;
            }
            return basicColor;
        }

        private static string GetStatusText(string statusText, string threeYearStatusText, out string color)
        {
            Func<Color, string> toColor = col => col.R.ToString("X2") + col.G.ToString("X2") + col.B.ToString("X2");

            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_ok)
            {
                color = toColor(Color.FromArgb(0, 100, 0));
                return Resources.TXT_STATUS_REPORT_ok;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_CHECK_DUE && threeYearStatusText == Resources.TXT_STATUS_REPORT_CHECK_DUE)
            {
                color = toColor(Color.FromArgb(255, 140, 0));
                return Resources.TXT_STATUS_REPORT_CHECK_DUE;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_EXPIRED && threeYearStatusText == Resources.TXT_STATUS_REPORT_EXPIRED)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_EXPIRED;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_UNKNOWN && threeYearStatusText == Resources.TXT_STATUS_REPORT_UNKNOWN)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_UNKNOWN;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_CHECK_DUE)
            {
                color = toColor(Color.FromArgb(255, 140, 0));
                return Resources.TXT_STATUS_REPORT_CHECK_DUE;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_EXPIRED)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_EXPIRED;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_UNKNOWN)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_UNKNOWN;
            }
            if (threeYearStatusText == Resources.TXT_STATUS_REPORT_ok && statusText == Resources.TXT_STATUS_REPORT_CHECK_DUE)
            {
                color = toColor(Color.FromArgb(255, 140, 0));
                return Resources.TXT_STATUS_REPORT_CHECK_DUE;
            }
            if (threeYearStatusText == Resources.TXT_STATUS_REPORT_ok && statusText == Resources.TXT_STATUS_REPORT_EXPIRED)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_EXPIRED;
            }
            if (threeYearStatusText == Resources.TXT_STATUS_REPORT_ok && statusText == Resources.TXT_STATUS_REPORT_UNKNOWN)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_UNKNOWN;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_EXPIRED && threeYearStatusText == Resources.TXT_STATUS_REPORT_CHECK_DUE)
            {
                color = toColor(Color.FromArgb(255, 140, 0));
                return Resources.TXT_STATUS_REPORT_CHECK_DUE;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_UNKNOWN && threeYearStatusText == Resources.TXT_STATUS_REPORT_EXPIRED)
            {
                color = toColor(Color.FromArgb(178, 34, 34));
                return Resources.TXT_STATUS_REPORT_EXPIRED;
            }
            color = toColor(Color.FromArgb(0, 0, 0));
            return string.Empty;
        }

        private static string GetStatus(ReportItemStatus itemStatus, out string statusText)
        {
            Color color;
            switch (itemStatus)
            {
                case ReportItemStatus.Ok:
                    color = Color.FromArgb(0, 100, 0);
                    statusText = Resources.TXT_STATUS_REPORT_ok;
                    break;
                case ReportItemStatus.CheckDue:
                    color = Color.FromArgb(255, 140, 0);
                    statusText = Resources.TXT_STATUS_REPORT_CHECK_DUE;
                    break;
                case ReportItemStatus.Expired:
                    color = Color.FromArgb(178, 34, 34);
                    statusText = Resources.TXT_STATUS_REPORT_EXPIRED;
                    break;
                default:
                    color = Color.FromArgb(178, 34, 34);
                    statusText = Resources.TXT_STATUS_REPORT_UNKNOWN;
                    break;
            }
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}
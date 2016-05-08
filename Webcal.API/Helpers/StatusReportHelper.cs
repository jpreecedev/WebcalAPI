namespace Webcal.API.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Model;

    public static class StatusReportHelper
    {
        private static readonly Random _random = new Random();

        public static StatusReportViewModel GenerateStatusReport(StatusReportData data)
        {
            var result = new StatusReportViewModel(data.Technicians, data.WorkshopSettings)
            {
                Performance = new List<StatusReportTechnician>()
            };

            GetTechniciansPerformanceData(data, result);

            result.LineChartLabels = data.Last12Months.Select(c => c.ToString("MMM yy")).ToList();

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
                result.Performance.Add(new StatusReportTechnician
                {
                    Value = technician.JobsDoneInLast12Months,
                    Color = GetColor(index),
                    Label = technician.Technician.Name.ToTitleCase()
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
    }
}
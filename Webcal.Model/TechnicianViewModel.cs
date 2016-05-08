namespace Webcal.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Connect.Shared.Models;

    public class TechnicianViewModel
    {
        public TechnicianViewModel(ICollection<ReportDocumentViewModel> documents, IEnumerable<DateTime> last12Months, Technician technician)
        {
            Technician = technician;
            JobsDoneInLast12Months = documents.Count(c => c.Technician == technician.Name);
            JobsMonthByMonth = new Dictionary<DateTime, int>();

            foreach (var last12Month in last12Months)
            {
                JobsMonthByMonth.Add(last12Month, 0);
            }

            var technicianDocuments = documents.Where(c => string.Equals(c.Technician, technician.Name, StringComparison.CurrentCultureIgnoreCase)).ToList();
            foreach (var viewModel in technicianDocuments)
            {
                var dt = new DateTime(viewModel.Created.Year, viewModel.Created.Month, 1);
                JobsMonthByMonth[dt] += 1;
            }
        }

        public Technician Technician { get; }

        public int JobsDoneInLast12Months { get; }

        public Dictionary<DateTime, int> JobsMonthByMonth { get; }
    }
}
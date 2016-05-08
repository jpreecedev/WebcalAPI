namespace Webcal.Model
{
    using System;
    using System.Collections.Generic;
    using Connect.Shared;
    using Connect.Shared.Models;

    public class StatusReportData
    {
        public List<Technician> Technicians { get; set; }
        public WorkshopSettings WorkshopSettings { get; set; }
        public List<DateTime> Last12Months { get; set; }
        public List<ReportDocumentViewModel> Documents { get; set; }
    }
}
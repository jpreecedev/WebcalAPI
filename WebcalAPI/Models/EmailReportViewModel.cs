namespace WebcalAPI.Models
{
    using System;
    using Core;

    public class EmailReportViewModel
    {
        public int UserId { get; set; }
        public string Recipient { get; set; }
        public ReportType ReportType { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
    }
}
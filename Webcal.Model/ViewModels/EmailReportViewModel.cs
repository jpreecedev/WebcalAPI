namespace Webcal.Model.ViewModels
{
    using System;

    public class EmailReportViewModel
    {
        public int UserId { get; set; }
        public string Recipient { get; set; }
        public ApiReportType ReportType { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
    }
}
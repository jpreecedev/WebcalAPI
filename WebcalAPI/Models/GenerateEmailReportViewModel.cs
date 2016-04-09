namespace WebcalAPI.Models
{
    using System;
    using System.Collections.Generic;

    public class GenerateEmailReportViewModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<ClientNameViewModel> Clients { get; set; }
    }
}
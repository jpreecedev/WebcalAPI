namespace Webcal.Model
{
    using System;

    public class StatusReportTechnician
    {
        public int Value { get; set; }
        public string Color { get; set; }
        public string Label { get; set; }

        public DateTime? DateOfNextCheck { get; set; }
        public DateTime? TrainingDateExpiry { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
        public string ThreeYearColor { get; set; }
    }
}
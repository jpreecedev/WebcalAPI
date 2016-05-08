namespace Webcal.Model
{
    using System.Collections.Generic;

    public class CalibrationEmailViewModel
    {
        public string Recipient { get; set; }

        public ICollection<RecentCalibrationsViewModel> Calibrations { get; set; }
    }
}
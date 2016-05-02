namespace Webcal.Model.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Connect.Shared.Models;

    public class InspectionDataViewModel
    {
        public InspectionDataViewModel()
        {
            History = new List<HistoryViewModel>();
        }

        public DateTime CalibrationDate { get; set; }

        public DateTime InspectionDate { get; set; }

        public string VehicleMake { get; set; }

        public string VehicleModel { get; set; }

        public string TachoModel { get; set; }

        public string InspectionData { get; set; }

        public List<HistoryViewModel> History { get; set; }

        public bool IsDefault
        {
            get
            {
                return CalibrationDate == default(DateTime) && InspectionDate == default(DateTime);
            }
        }

        public void Parse(TachographDocument document)
        {
            if (document == null)
            {
                return;
            }

            CalibrationDate = document.CalibrationTime.GetValueOrDefault();
            InspectionDate = document.InspectionDate.GetValueOrDefault();
            VehicleMake = document.VehicleMake;
            VehicleModel = document.VehicleModel;
            TachoModel = document.TachographModel;

            if (!string.IsNullOrEmpty(document.InspectionInfo))
            {
                InspectionData = document.InspectionInfo;
            }
        }
    }
}
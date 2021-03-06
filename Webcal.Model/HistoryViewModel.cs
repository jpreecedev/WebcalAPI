﻿namespace Webcal.Model
{
    using System;

    public class HistoryViewModel
    {
        public HistoryViewModel(DateTime calibrationDate, string inspectionInfo)
        {
            CalibrationDate = calibrationDate;
            InspectionInfo = inspectionInfo;
        }

        public DateTime CalibrationDate { get; set; }
        public string InspectionInfo { get; set; }
    }
}
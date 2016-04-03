namespace WebcalAPI.Models
{
    using System;
    using Connect.Shared;
    using Connect.Shared.Models;

    public class CalibrationsDueViewModel
    {
        public CalibrationsDueViewModel(Document document)
        {
            if (document == null)
            {
                return;
            }

            DocumentId = document.Id;
            DocumentTypeEnum = DocumentTypeHelper.Parse(document);
            DocumentType = DocumentTypeEnum.AsDisplayString();
            DocumentIcon = DocumentType.Replace(" ", "");
            Date = document.InspectionDate.GetValueOrDefault();
            Expiration = document.InspectionDate.GetValueOrDefault().AddYears(2);
            Registration = document.RegistrationNumber;
            Technician = document.Technician;
            Customer = document.CustomerContact;
            DepotName = document.DepotName;

            var tachographDocument = document as TachographDocument;
            VehicleManufacturer = tachographDocument != null ? tachographDocument.VehicleMake : string.Empty;
        }

        public string DepotName { get; set; }

        public DateTime Date { get; set; }

        public DateTime Expiration { get; set; }

        public string Registration { get; set; }

        public string Technician { get; set; }

        public string Customer { get; set; }

        public string VehicleManufacturer { get; set; }

        public int DocumentId { get; set; }

        public FilterDocumentType DocumentTypeEnum { get; set; }

        public string DocumentType { get; set; }

        public string DocumentIcon { get; set; }
    }
}
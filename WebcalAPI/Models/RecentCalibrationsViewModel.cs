namespace WebcalAPI.Models
{
    using System;
    using Connect.Shared;
    using Connect.Shared.Models;

    public class RecentCalibrationsViewModel
    {
        public RecentCalibrationsViewModel(Document document)
        {
            if (document == null)
            {
                return;
            }

            DocumentId = document.Id;
            CompanyName = document.CompanyName;
            DocumentTypeEnum = DocumentTypeHelper.Parse(document);
            DocumentType = DocumentTypeEnum.AsDisplayString();
            DocumentIcon = DocumentType.Replace(" ", "");
            Expiration = document.InspectionDate.GetValueOrDefault().Date.AddYears(2);
            Registration = document.RegistrationNumber;
            Technician = document.Technician;
            Customer = document.CustomerContact;
            DepotName = document.DepotName;
        }

        public FilterDocumentType DocumentTypeEnum { get; set; }

        public int DocumentId { get; set; }

        public string CompanyName { get; set; }

        public string DocumentType { get; set; }

        public DateTime Expiration { get; set; }

        public string Registration { get; set; }

        public string Technician { get; set; }

        public string Customer { get; set; }

        public string DocumentIcon { get; set; }

        public string DepotName { get; set; }
    }
}
namespace Webcal.Model
{
    using System;
    using System.Xml.Serialization;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Newtonsoft.Json;

    public class RecentCalibrationsViewModel
    {
        public RecentCalibrationsViewModel(Document document)
        {
            if (document == null)
            {
                return;
            }

            UserId = document.UserId;
            Created = document.Created;
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

        [JsonIgnore, XmlIgnore]
        public int UserId { get; set; }

        [JsonIgnore, XmlIgnore]
        public DateTime Created { get; set; }

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
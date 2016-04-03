namespace WebcalAPI.Models
{
    using System;
    using Connect.Shared;
    using Connect.Shared.Models;

    public class CentreCheckViewModel
    {
        public CentreCheckViewModel(QCReport6Month qcReport6Month)
        {
            if (qcReport6Month == null)
            {
                return;
            }

            CentreName = qcReport6Month.CentreName;
            SealNumber = qcReport6Month.CentreSealNumber;
            Date = qcReport6Month.Date;
            Name = qcReport6Month.Name;
            DocumentId = qcReport6Month.Id;
            DocumentTypeEnum = DocumentType.QCReport6Month;
        }

        public string CentreName { get; set; }
        public string SealNumber { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int DocumentId { get; set; }
        public DocumentType DocumentTypeEnum { get; set; }
    }
}
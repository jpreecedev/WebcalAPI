namespace Webcal.Model
{
    using System;
    using Connect.Shared;
    using Connect.Shared.Models;

    public class QCCheckViewModel
    {
        public QCCheckViewModel(QCReport qcReport)
        {
            if (qcReport == null)
            {
                return;
            }

            CentreName = qcReport.TachoCentreName;
            Date = qcReport.DateOfAudit.Date;
            ThreeChecksCompleted = qcReport.ThreeBasicChecksCompleted;
            ManagerName = qcReport.QCManagerName;
            Technician = qcReport.TechnicianName;
            DocumentId = qcReport.Id;
            DocumentTypeEnum = DocumentType.QCReport;
        }

        public string CentreName { get; set; }

        public DateTime Date { get; set; }

        public bool ThreeChecksCompleted { get; set; }

        public string ManagerName { get; set; }

        public string Technician { get; set; }

        public int DocumentId { get; set; }

        public DocumentType DocumentTypeEnum { get; set; }
    }
}
namespace Webcal.Model.ViewModels
{
    using Connect.Shared;

    public class CalibrationCertificateEmailViewModel
    {
        public string Recipient { get; set; }

        public int DocumentId { get; set; }

        public DocumentType DocumentType { get; set; }
    }
}
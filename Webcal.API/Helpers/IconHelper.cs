using System;

namespace Webcal.API.Helpers
{
    using Connect.Shared.Models;

    public static class IconHelper
    {
        public static string GetIcon(FilterDocumentType documentType)
        {
            switch (documentType)
            {
                case FilterDocumentType.AnalogueTachograph:
                    return "http://test.webcalconnect.com/img/AnalogueTacho.png";
                case FilterDocumentType.Tachograph:
                    return "http://test.webcalconnect.com/img/DigitalTacho.png";
                case FilterDocumentType.Undownloadability:
                    return "http://test.webcalconnect.com/img/Undownloadability.png";
                case FilterDocumentType.LetterForDecommissioning:
                    return "http://test.webcalconnect.com/img/LetterForDecomm.png";
                default:
                    throw new ArgumentOutOfRangeException(nameof(documentType), documentType, null);
            }
        }

        public static string GetQCCheck()
        {
            return "http://test.webcalconnect.com/img/qccheck.png";
        }

        public static string GetCentreCheck()
        {
            return "http://test.webcalconnect.com/img/centrecheck.png";
        }
    }
}
namespace WebcalAPI.Models
{
    using System;
    using Connect.Shared.Models.License;

    public class LicenseViewModel
    {
        public LicenseViewModel()
        {
        }

        public LicenseViewModel(License license)
        {
            if (license == null) throw new ArgumentNullException(nameof(license));

            License = license.Key;
            Expiration = license.Expiration;
            AccessId = license.AccessId;
        }

        public Guid AccessId { get; set; }

        public DateTime Expiration { get; set; }

        public string License { get; set; }

        public bool HasExpired
        {
            get { return Expiration < DateTime.Now; }
        }
    }
}
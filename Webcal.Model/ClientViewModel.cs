namespace Webcal.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Connect.Shared.Models.License;

    public class ClientViewModel
    {
        public ClientViewModel()
        {
            
        }

        public ClientViewModel(Client client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            Name = client.Name;
            Licenses = client.Licenses != null ? client.Licenses.Select(c => new LicenseViewModel(c)).ToList() : new List<LicenseViewModel>();
            AccessId = client.AccessId;
        }

        public Guid AccessId { get; set; }

        public List<LicenseViewModel> Licenses { get; set; }

        public string Name { get; set; }
    }
}
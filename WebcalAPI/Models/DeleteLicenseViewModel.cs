﻿namespace WebcalAPI.Models
{
    using System;

    public class DeleteLicenseViewModel
    {
        public Guid ClientAccessId { get; set; }
        public Guid LicenseAccessId { get; set; }
    }
}
namespace WebcalAPI.Models
{
    using System;

    public class RegisterUserViewModel
    {
        public string EmailAddress { get; set; }
        public string CompanyName { get; set; }
        public DateTime Expiration { get; set; }
        public string Password { get; set; }
    }
}
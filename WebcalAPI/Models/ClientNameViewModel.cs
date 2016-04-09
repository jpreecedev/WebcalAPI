namespace WebcalAPI.Models
{
    using System;
    using Connect.Shared.Models;

    public class ClientNameViewModel
    {
        public ClientNameViewModel(ConnectUser connectUser)
        {
            if (connectUser == null) throw new ArgumentNullException(nameof(connectUser));

            Name = connectUser.CompanyKey;
            Id = connectUser.Id;
        }

        public ClientNameViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
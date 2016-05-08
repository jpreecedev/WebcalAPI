namespace Webcal.API.Core
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Threading.Tasks;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Connect.Shared.Models.License;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class ConnectContext : IdentityDbContext<ConnectUser, ConnectRole, int, ConnectUserLogin, ConnectUserRole, ConnectUserClaim>, IConnectContext
    {
        public ConnectContext()
            : base("ConnectContext")
        {
        }

        public DbSet<TachographDocument> TachographDocuments { get; set; }

        public DbSet<UndownloadabilityDocument> UndownloadabilityDocuments { get; set; }

        public DbSet<LetterForDecommissioningDocument> LetterForDecommissioningDocuments { get; set; }

        public DbSet<UserPendingAuthorization> UnauthorizedUsers { get; set; }

        public DbSet<ConnectUserNode> UserNodes { get; set; }

        public DbSet<Settings> Settings { get; set; }

        public DbSet<TachoFleetEmail> Emails { get; set; }

        public DbSet<TachoCentreOperator> TachoCentreOperators { get; set; }

        public DbSet<CustomerContact> CustomerContacts { get; set; }

        public DbSet<LinkedVehicle> LinkedVehicles { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<License> Licenses { get; set; }

        public DbSet<MobileApplicationUser> MobileApplicationUsers { get; set; }

        public DbSet<QCReport> QCReports { get; set; }

        public DbSet<QCReport6Month> QCReports6Month { get; set; }

        public DbSet<Technician> Technicians { get; set; }

        public DbSet<WorkshopSettings> WorkshopSettings { get; set; }

        public override Task<int> SaveChangesAsync()
        {
            try
            {
                return base.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new Exception(fullErrorMessage);
            }
        }

        public static ConnectContext Create()
        {
            return new ConnectContext();
        }
    }
}
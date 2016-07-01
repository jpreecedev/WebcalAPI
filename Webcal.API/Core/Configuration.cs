namespace Webcal.API.Core
{
    using System.Data.Entity.Migrations;
    using Shared;

    public class Configuration : DbMigrationsConfiguration<ConnectContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
            ContextKey = "Connect.Shared.Migrations.Configuration";
        }
    }
}
namespace WebcalAPI.Core
{
    using System.Data.Entity.Migrations;

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
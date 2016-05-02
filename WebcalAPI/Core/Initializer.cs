namespace Webcal.API.Core
{
    using System.Data.Entity;

    public class Initializer : MigrateDatabaseToLatestVersion<ConnectContext, Configuration>
    {
    }
}
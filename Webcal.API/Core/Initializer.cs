namespace Webcal.API.Core
{
    using System.Data.Entity;
    using Shared;

    public class Initializer : MigrateDatabaseToLatestVersion<ConnectContext, Configuration>
    {
    }
}
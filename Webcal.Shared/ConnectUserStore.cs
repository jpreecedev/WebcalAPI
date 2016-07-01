namespace Webcal.Shared
{
    using System.Data.Entity;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class ConnectUserStore : UserStore<ConnectUser, ConnectRole, int, ConnectUserLogin, ConnectUserRole, ConnectUserClaim>
    {
        public ConnectUserStore(DbContext context)
            : base(context)
        {
        }
    }
}
namespace WebcalAPI.Core
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Security.Principal;
    using Connect.Shared.Models;
    using Microsoft.AspNet.Identity;

    public static class Extensions
    {
        public static ConnectUser GetConnectUser(this IPrincipal principal)
        {
            if (principal == null || principal.Identity == null)
            {
                return null;
            }

            var userId = principal.Identity.GetUserId<int>();
            if (userId < 1)
            {
                return null;
            }

            using (var context = new ConnectContext())
            {
                return context.Users.Include(c => c.CustomerContact)
                    .FirstOrDefault(c => c.Id == userId);
            }
        }
    }
}
namespace Webcal.Shared
{
    using Connect.Shared.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.DataProtection;

    public class ApplicationUserManager : UserManager<ConnectUser, int>
    {
        public ApplicationUserManager(IUserStore<ConnectUser, int> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var provider = new DpapiDataProtectionProvider("WebcalAPI");

            return new ApplicationUserManager(new ConnectUserStore(context.Get<ConnectContext>()))
            {
                UserTokenProvider = new DataProtectorTokenProvider<ConnectUser, int>(provider.Create("EmailConfirmation"))
            };
        }
    }
}
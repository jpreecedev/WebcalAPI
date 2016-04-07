namespace WebcalAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Transactions;
    using System.Web.Http;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Connect.Shared.Models.License;
    using Core;
    using Microsoft.AspNet.Identity;
    using Models;

    [Authorize(Roles = "Administrator")]
    [RoutePrefix("api/registeruser")]
    public class RegisterUserController : BaseApiController
    {
        [HttpPost]
        public IHttpActionResult Post([FromBody]RegisterUserViewModel data) //Do not make async until upgrading to 4.5.1
        {
            using (var context = new ConnectContext())
            {
                var user = new ConnectUser();
                user.UserName = user.Email = data.EmailAddress;
                user.CompanyKey = data.CompanyName;
                user.IsAuthorized = true;
                user.LicenseKey = Convert.ToInt32(data.Expiration.Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd(char.Parse("0")));
                user.CustomerContact = new CustomerContact();

                var company = context.Users.FirstOrDefault(c => c.CompanyKey == user.CompanyKey);
                if (company != null)
                {
                    return BadRequest("The company name is already in use.");
                }

                var uniquePassword = Guid.NewGuid().ToString().Replace("-", "");
                uniquePassword = uniquePassword.Substring(0, 10).ToUpper() + uniquePassword.Substring(10).ToLower();

                using (var transaction = new TransactionScope())
                {
                    var result = UserManager.Create(user, uniquePassword);
                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(user.Id, ConnectRoles.TachographCentre);
                        context.SaveChanges();

                        SendConfirmationEmail(user);
                        AssociateUserWithTachoCentre(context, user);
                        AddLicenseForUser(context, user, data);

                        transaction.Complete();
                        return Ok();
                    }
                    return BadRequest(string.Join(",", result.Errors));
                }
            }
        }

        private void SendConfirmationEmail(ConnectUser user)
        {
            var code = UserManager.GenerateEmailConfirmationToken(user.Id);

#if DEBUG
            var callbackUrl = $"http://localhost:3000/confirm-account/{code}";
#else
            var callbackUrl = $"http://test.webcalconnect.com/confirm-account/{code}";
#endif

            SendEmail(user.Email, "Please confirm your email address", $"<a href=\"{callbackUrl}\">Click Here To Verify Your Email Address</a>");
        }

        private void AssociateUserWithTachoCentre(ConnectContext context, ConnectUser user)
        {
            var userId = User.Identity.GetUserId<int>();
            var tachographCentreUser = context.Users.First(c => c.Id == userId);
            var operatorUser = context.Users.First(c => c.Id == user.Id);

            var existingLink = context.TachoCentreOperators.Include(c => c.OperatorUser)
                .Include(c => c.TachographCentreUser)
                .Any(c => c.TachographCentreUser.Id == tachographCentreUser.Id && c.OperatorUser.Id == operatorUser.Id);

            if (!existingLink)
            {
                context.TachoCentreOperators.Add(new TachoCentreOperator
                {
                    OperatorUser = operatorUser,
                    TachographCentreUser = tachographCentreUser
                });

                context.SaveChanges();
            }
        }

        private static void AddLicenseForUser(ConnectContext context, ConnectUser user, RegisterUserViewModel data)
        {
            var licenseUser = context.Clients.Include(x => x.Licenses).FirstOrDefault(c => c.Name == user.CompanyKey);
            if (licenseUser == null)
            {
                licenseUser = new Client
                {
                    Name = user.CompanyKey,
                    Created = DateTime.Now,
                    AccessId = Guid.NewGuid(),
                    Licenses = new List<License>()
                };
                context.Clients.Add(licenseUser);
            }

            licenseUser.Licenses.Add(new License
            {
                Created = DateTime.Now,
                Expiration = data.Expiration,
                Key = data.Expiration.Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd(char.Parse("0")),
                AccessId = Guid.NewGuid()
            });

            context.SaveChanges();
        }
    }
}
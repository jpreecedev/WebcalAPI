namespace Webcal.API.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Connect.Shared;
    using Core;
    using Microsoft.AspNet.Identity;
    using Model;

    [RoutePrefix("api/addressbook")]
    public class AddressBookController : BaseApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var userId = User.Identity.GetUserId<int>();
            var isAdministrator = User.IsInRole(ConnectRoles.Admin);

            using (var context = new ConnectContext())
            {
                var addressEntries = context.CustomerContacts.Where(c => c.UserId == userId || isAdministrator)
                    .Where(c => c.UserId != 0 && c.Name != null)
                    .OrderBy(c => c.Name)
                    .Select(c => new AddressBookEntryViewModel
                    {
                        Id = c.Id,
                        Email = c.Email,
                        Name = c.Name,
                        Address = c.Address,
                        SecondaryEmail = c.SecondaryEmail
                    });

                return Ok(await addressEntries.ToListAsync());
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post(AddressBookEntryViewModel addressBookEntry)
        {
            if (addressBookEntry == null)
            {
                return BadRequest();
            }

            using (var context = new ConnectContext())
            {
                var userId = User.Identity.GetUserId<int>();
                var isAdministrator = User.IsInRole(ConnectRoles.Admin);

                var entry = await context.CustomerContacts.FirstOrDefaultAsync(c => c.Id == addressBookEntry.Id && (userId == c.UserId || isAdministrator));
                if (entry != null)
                {
                    entry.Email = addressBookEntry.Email;
                    entry.SecondaryEmail = addressBookEntry.SecondaryEmail;

                    await context.SaveChangesAsync();
                    return Ok();
                }
            }

            return NotFound();
        }
    }
}
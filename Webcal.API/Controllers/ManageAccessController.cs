namespace Webcal.API.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using Core;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Model;
    using Shared;

    [Authorize(Roles = "Administrator")]
    [RoutePrefix("api/manageaccess")]
    public class ManageAccessController : BaseApiController
    {
        [HttpGet]
        [Route("users")]
        public async Task<IHttpActionResult> GetUsers()
        {
            using (var context = new ConnectContext())
            {
                var users = context.Users.Where(u => u.Deleted == null)
                    .OrderBy(c => c.CompanyKey)
                    .Select(c => new ManageAccessUserViewModel {Name = c.CompanyKey, Id = c.Id});

                return Ok(await users.ToListAsync());
            }
        }

        [HttpGet]
        [Route("sites/{userId:int}")]
        public async Task<IHttpActionResult> GetSites(int userId)
        {
            if (userId < 1)
            {
                return BadRequest();
            }

            using (var context = new ConnectContext())
            {
                var connectedSites = context.UserNodes.Include(x => x.ConnectUser)
                    .Where(c => c.Deleted == null && c.ConnectUser.Id == userId)
                    .Select(c => new ManageAccessSiteViewModel
                    {
                        Description = c.CompanyKey + "-" + c.MachineKey + "-" + c.LicenseKey,
                        IsRevoked = !c.IsAuthorized
                    });

                return Ok(await connectedSites.ToListAsync());
            }
        }

        [HttpPost]
        [Route("sites")]
        public async Task<IHttpActionResult> Post(ManageAccessSiteViewModel selectedSite)
        {
            if (selectedSite == null)
            {
                return BadRequest();
            }

            using (var context = new ConnectContext())
            {
                var site = await context.UserNodes.FirstOrDefaultAsync(c => c.CompanyKey + "-" + c.MachineKey + "-" + c.LicenseKey == selectedSite.Description);
                if (site != null)
                {
                    site.IsAuthorized = !site.IsAuthorized;
                    await context.SaveChangesAsync();
                    return Ok();
                }
            }

            return NotFound();
        }
    }
}
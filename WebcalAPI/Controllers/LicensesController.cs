namespace WebcalAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Connect.Shared.Models.License;
    using Core;
    using Models;

    [Authorize(Roles = "Administrator")]
    [RoutePrefix("api/licenses")]
    public class LicensesController : BaseApiController
    {
        [HttpGet]
        [Route("clients")]
        public async Task<IHttpActionResult> GetClients()
        {
            using (var context = new ConnectContext())
            {
                var model = context.Clients.Where(c => c.Deleted == null).Include(x => x.Licenses).Select(client => new ClientViewModel
                {
                    AccessId = client.AccessId,
                    Name = client.Name,
                    Licenses = client.Licenses.OrderByDescending(c => c.Expiration).Where(c => c.Deleted == null).Select(license => new LicenseViewModel
                    {
                        License = license.Key,
                        Expiration = license.Expiration,
                        AccessId = license.AccessId
                    }).ToList()
                });

                return Ok(await model.OrderBy(c => c.Name).ToListAsync());
            }
        }

        [HttpPost]
        [Route("client")]
        public async Task<IHttpActionResult> AddClient([FromBody]ClientViewModel data)
        {
            if (data == null || string.IsNullOrEmpty(data.Name))
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (var context = new ConnectContext())
            {
                if (context.Clients.Any(c => c.Name == data.Name))
                {
                    return BadRequest("A client already exists with that name.");
                }

                var client = new Client
                {
                    Name = data.Name,
                    Created = DateTime.Now,
                    AccessId = Guid.NewGuid(),
                    Licenses = new List<License>()
                };

                context.Clients.Add(client);
                await context.SaveChangesAsync();

                return Ok(client);
            }
        }

        [HttpDelete]
        [Route("client")]
        public async Task<IHttpActionResult> DeleteClient([FromBody]DeleteLicenseViewModel data)
        {
            using (var context = new ConnectContext())
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.AccessId == data.ClientAccessId);
                if (client != null)
                {
                    client.Deleted = DateTime.Now;
                    await context.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();
            }
        }

        [HttpDelete]
        [Route("license")]
        public async Task<IHttpActionResult> DeleteLicense([FromBody]DeleteLicenseViewModel data)
        {
            using (var context = new ConnectContext())
            {
                var client = await context.Clients.Include(x => x.Licenses).FirstOrDefaultAsync(c => c.AccessId == data.ClientAccessId);
                if (client != null)
                {
                    var license = client.Licenses.FirstOrDefault(c => c.AccessId == data.LicenseAccessId);
                    if (license != null)
                    {
                        license.Deleted = DateTime.Now;
                        await context.SaveChangesAsync();
                        return Ok();
                    }
                }
                return NotFound();
            }
        }

        [HttpPost]
        [Route("license")]
        public async Task<IHttpActionResult> AddLicense([FromBody]LicenseViewModel data)
        {
            using (var context = new ConnectContext())
            {
                var client = await context.Clients.Include(x => x.Licenses).FirstOrDefaultAsync(c => c.AccessId == data.AccessId);
                if (client != null)
                {
                    var license = new License
                    {
                        Created = DateTime.Now,
                        Expiration = data.Expiration,
                        Key = data.Expiration.Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd(char.Parse("0")),
                        AccessId = Guid.NewGuid()
                    };

                    client.Licenses.Add(license);

                    await context.SaveChangesAsync();

                    return Ok(new LicenseViewModel(license));
                }

                return BadRequest("Could not find client");
            }
        }

        [HttpGet]
        [Route("license/{expiration}")]
        public IHttpActionResult GetLicense(DateTime expiration)
        {
            return Ok(new
            {
                key = expiration.Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd(char.Parse("0"))
            });
        }
    }
}
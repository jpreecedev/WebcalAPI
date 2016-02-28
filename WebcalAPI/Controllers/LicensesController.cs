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

    [RoutePrefix("api/licenses")]
    public class LicensesController : ApiController
    {
        [HttpGet]
        [Route("clients")]
        public async Task<IHttpActionResult> GetClients()
        {
            using (var context = new ConnectContext())
            {
                var model = context.Clients.Where(c => c.Deleted == null).Include(x => x.Licenses).Select(client => new
                {
                    ClientId = client.AccessId,
                    ClientName = client.Name,
                    Licenses = client.Licenses.OrderByDescending(c => c.Expiration).Where(c => c.Deleted == null).Select(license => new
                    {
                        License = license.Key,
                        license.Expiration,
                        LicenseId = license.AccessId
                    }).ToList()
                });

                return Ok(await model.OrderBy(c => c.ClientName).ToListAsync());
            }
        }

        [HttpDelete]
        [Route("client/{accessId}")]
        public async Task DeleteClient(Guid accessId)
        {
            using (var context = new ConnectContext())
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.AccessId == accessId);
                if (client != null)
                {
                    client.Deleted = DateTime.Now;
                    await context.SaveChangesAsync();
                }
            }
        }

        [HttpPost]
        [Route("client/{name}")]
        public async Task<IHttpActionResult> AddClient(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (var context = new ConnectContext())
            {
                var client = new Client
                {
                    Name = name,
                    Created = DateTime.Now,
                    AccessId = Guid.NewGuid(),
                    Licenses = new List<License>()
                };

                context.Clients.Add(client);
                await context.SaveChangesAsync();

                return Json(new {client});
            }
        }

        [HttpDelete]
        [Route("license/{accessId}")]
        public async Task<IHttpActionResult> DeleteLicense(Guid accessId)
        {
            using (var context = new ConnectContext())
            {
                var license = await context.Licenses.FirstOrDefaultAsync(c => c.AccessId == accessId);
                if (license != null)
                {
                    license.Deleted = DateTime.Now;
                    await context.SaveChangesAsync();
                }

                return Ok();
            }
        }

        [HttpPost]
        [Route("license")]
        public async Task<IHttpActionResult> AddLicense([FromBody] Guid accessId, [FromBody] DateTime expiration)
        {
            using (var context = new ConnectContext())
            {
                var client = await context.Clients.Include(x => x.Licenses).FirstOrDefaultAsync(c => c.AccessId == accessId);
                if (client != null)
                {
                    var license = new License
                    {
                        Created = DateTime.Now,
                        Expiration = expiration,
                        Key = expiration.Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd(char.Parse("0")),
                        AccessId = Guid.NewGuid()
                    };

                    client.Licenses.Add(license);

                    await context.SaveChangesAsync();

                    return Json(new {license});
                }

                return BadRequest("Could not find client");
            }
        }

        [HttpGet]
        [Route("license/{expiration}")]
        public IHttpActionResult GetLicense(DateTime expiration)
        {
            return Json(new
            {
                key = expiration.Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd(char.Parse("0"))
            });
        }
    }
}
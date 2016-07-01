namespace Webcal.API.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Connect.Shared;
    using Core;
    using Model;
    using Shared;

    [Authorize(Roles = ConnectRoles.Admin)]
    public class DetailedExceptionsController : BaseApiController
    {
        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody]DeleteDetailedExceptionViewModel data)
        {
            using (var context = new ConnectContext())
            {
                var exception = context.DetailedExceptions.FirstOrDefault(u => u.Id == data.Id);
                if (exception != null)
                {
                    exception.Deleted = DateTime.Now;
                    await context.SaveChangesAsync();
                    return Ok();
                }
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            using (var context = new ConnectContext())
            {
                var exceptions = context.DetailedExceptions.Where(u => u.Deleted == null)
                    .Join(context.Users, exception => exception.UserId, user => user.Id, (exception, user) => new { ConnectUser = user, Exception = exception })
                    .OrderByDescending(c => c.Exception.Occurred)
                    .Select(c => new DetailedExceptionViewModel
                    {
                        Id = c.Exception.Id,
                        Date = c.Exception.Occurred,
                        Message = c.Exception.ExceptionDetails,
                        Company = c.ConnectUser.CompanyKey
                    });

                return Ok(await exceptions.ToListAsync());
            }
        }
    }
}
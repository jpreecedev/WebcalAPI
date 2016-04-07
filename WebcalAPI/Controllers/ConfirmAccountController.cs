namespace WebcalAPI.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Models;

    [AllowAnonymous]
    [RoutePrefix("api/confirmaccount")]
    public class ConfirmAccountController : BaseApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]ConfirmAccountViewModel data)
        {
            var result = await UserManager.ConfirmEmailAsync(data.UserId, data.Code);
            if (result.Succeeded)
            {
                await UserManager.RemovePasswordAsync(data.UserId);
                await UserManager.AddPasswordAsync(data.UserId, data.Password);
                return Ok();
            }
            return BadRequest(string.Join(",", result.Errors));
        }
    }
}
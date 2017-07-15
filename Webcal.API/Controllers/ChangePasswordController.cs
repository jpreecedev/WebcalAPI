namespace Webcal.API.Controllers
{
    using System.Threading.Tasks;
    using System.Transactions;
    using System.Web.Http;
    using Core;
    using Model;

    [RoutePrefix("api/changepassword")]
    public class ChangePasswordController : BaseApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] ChangePasswordViewModel data)
        {
            using (var context = new ConnectContext())
            {
                var result = await UserManager.ChangePasswordAsync(ConnectUser.Id, data.CurrentPassword, data.NewPassword);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(string.Join(",", result.Errors));
            }
        }
    }
}
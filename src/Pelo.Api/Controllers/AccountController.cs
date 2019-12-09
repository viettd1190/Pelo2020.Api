using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Account;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IHttpContextAccessor _accessor;

        public AccountController(IAccountService accountService, IHttpContextAccessor accessor) : base(accountService)
        {
            _accessor = accessor;
        }

        /// <summary>
        ///     Đăng nhập
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/account/logon")]
        public async Task<ActionResult<LogonResponse>> LogOn(LogonRequest request)
        {
            var userAgent = string.Empty;
            if (Request.Headers.ContainsKey("User-Agent")) userAgent = Request.Headers["User-Agent"];

            var ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            return Ok(await AccountService.Logon(request, userAgent, ipAddress));
        }
    }
}
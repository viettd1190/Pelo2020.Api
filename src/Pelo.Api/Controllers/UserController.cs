using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.User;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IAccountService accountService,
                              IUserService userService) : base(accountService)
        {
            _userService = userService;
        }

        /// <summary>
        ///     Lấy danh sách user có phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user")]
        public async Task<ActionResult<GetUserPagingResponse>> GetByPaging([FromQuery] GetUserPagingRequest request)
        {
            return Ok(await _userService.GetPaging(await GetUserId(),
                                                   request));
        }

        /// <summary>
        ///     Xóa user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/user/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _userService.Delete(await GetUserId(),
                                                id));
        }
    }
}

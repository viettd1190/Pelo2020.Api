using System.Collections.Generic;
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
        ///     Thêm mới nhân viên
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/user")]
        public async Task<ActionResult<bool>> Insert(InsertUserRequest request)
        {
            return Ok(await _userService.Insert(await GetUserId(),
                                                request));
        }

        /// <summary>
        ///     Lấy thông tin nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/{id}")]
        public async Task<ActionResult<GetUserByIdResponse>> GetById(int id)
        {
            return Ok(await _userService.GetById(await GetUserId(),
                                                 id));
        }

        /// <summary>
        ///     Cập nhật thông tin nhân viên
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/user")]
        public async Task<ActionResult<bool>> Update(UpdateUserRequest request)
        {
            return Ok(await _userService.Update(await GetUserId(),
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

        /// <summary>
        ///     Lấy danh sách tất cả user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/all")]
        public async Task<ActionResult<IEnumerable<UserDisplaySimpleModel>>> GetAll()
        {
            return Ok(await _userService.GetAll());
        }

        /// <summary>
        ///     Kiểm tra xem user có thuộc quyền được xem tất cả crm hay không
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/crm_default")]
        public async Task<ActionResult<bool>> IsBelongCrmDefaultRole()
        {
            return Ok(await _userService.IsBelongDefaultCrmRole(await GetUserId()));
        }

        /// <summary>
        ///     Kiểm tra xem user có thuộc quyền được xem tất cả đơn hàng hay không
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/invoice_default")]
        public async Task<ActionResult<bool>> IsBelongInvoiceDefaultRole()
        {
            return Ok(await _userService.IsBelongDefaultInvoiceRole(await GetUserId()));
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Role;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IAccountService accountService,
                              IRoleService roleService) : base(accountService)
        {
            _roleService = roleService;
        }

        /// <summary>
        ///     Lấy tất cả role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/role/all")]
        public async Task<ActionResult<IEnumerable<RoleSimpleModel>>> GetAll()
        {
            return Ok(await _roleService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy tất cả role có phân trang
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/role")]
        public async Task<ActionResult<PageResult<GetRolePagingResponse>>> GetPaging([FromQuery] GetRolePagingRequest request)
        {
            return Ok(await _roleService.GetPaging(await GetUserId(), request));
        }

        /// <summary>
        ///     Lấy role by id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/role/{id}")]
        public async Task<ActionResult<GetRoleReponse>> GetById(int id)
        {
            return Ok(await _roleService.GetById(await GetUserId(), id));
        }

        /// <summary>
        ///     insert role
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/role")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertRole request)
        {
            return Ok(await _roleService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update role
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/role")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateRole request)
        {
            return Ok(await _roleService.Update(await GetUserId(), request));
        }

        /// <summary>
        ///     delete role
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/role/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _roleService.Delete(await GetUserId(), id));
        }

        /// <summary>
        ///     Lấy role name của user đăng nhập
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/role/get_current_role")]
        public async Task<ActionResult<TResponse<string>>> GetCurrentRole()
        {
            return Ok(await _roleService.GetNameByUserId(await GetUserId()));
        }
    }
}

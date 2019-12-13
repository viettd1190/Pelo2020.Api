using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Role;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IAccountService accountService, IRoleService roleService) : base(accountService)
        {
            _roleService = roleService;
        }

        /// <summary>
        ///     Lấy tất cả nhóm người dùng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/role/all")]
        public async Task<ActionResult<IEnumerable<RoleSimpleModel>>> GetAll()
        {
            return Ok(await _roleService.GetAll(await GetUserId()));
        }
    }
}
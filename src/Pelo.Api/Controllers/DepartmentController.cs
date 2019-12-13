using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Department;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IAccountService accountService,
                                    IDepartmentService departmentService) : base(accountService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        ///     Lấy tất cả phòng ban
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/department/all")]
        public async Task<ActionResult<IEnumerable<DepartmentSimpleModel>>> GetAll()
        {
            return Ok(await _departmentService.GetAll(await GetUserId()));
        }
    }
}

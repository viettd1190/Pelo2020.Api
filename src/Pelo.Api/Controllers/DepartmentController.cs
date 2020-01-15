using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Department;
using Pelo.Common.Models;

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

        /// <summary>
        ///     Lấy tất cả phòng ban
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/department")]
        public async Task<ActionResult<PageResult<GetDepartmentPagingResponse>>> GetPaging([FromQuery] GetDepartmentPagingRequest request)
        {
            return Ok(await _departmentService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy phòng ban
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/department/{id}")]
        public async Task<ActionResult<GetDepartmentReponse>> GetById(int id)
        {
            return Ok(await _departmentService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert phòng ban
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/department")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertDepartment request)
        {
            return Ok(await _departmentService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update phòng ban
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/department")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateDepartment request)
        {
            return Ok(await _departmentService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete phòng ban
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/department/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _departmentService.Delete(await GetUserId(), id));
        }
    }
}

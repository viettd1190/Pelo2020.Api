using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.TaskServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.TaskType;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class TaskTypeController : BaseController
    {
        private readonly ITaskTypeService _TaskTypeService;

        public TaskTypeController(IAccountService accountService,
                                   ITaskTypeService TaskTypeService) : base(accountService)
        {
            _TaskTypeService = TaskTypeService;
        }

        /// <summary>
        ///     Lấy tất cả type Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_type/all")]
        public async Task<ActionResult<IEnumerable<TaskTypeSimpleModel>>> GetAll()
        {
            return Ok(await _TaskTypeService.GetAll(await GetUserId()));
        }
        /// <summary>
        ///     Lấy tất cả type Task/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_type")]
        public async Task<ActionResult<GetTaskTypePagingResponse>> GetPaging([FromQuery] GetTaskTypePagingRequest request)
        {
            return Ok(await _TaskTypeService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy type Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_type/{id}")]
        public async Task<ActionResult<TaskTypeSimpleModel>> GetById(int id)
        {
            return Ok(await _TaskTypeService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert type Task
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/task_type")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertTaskType request)
        {
            return Ok(await _TaskTypeService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert type Task
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/task_type")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateTaskType request)
        {
            return Ok(await _TaskTypeService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete type Task
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/task_type/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _TaskTypeService.Delete(await GetUserId(), id));
        }
    }
}

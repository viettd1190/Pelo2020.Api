using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.TaskServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.TaskPriority;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class TaskPriorityController : BaseController
    {
        private readonly ITaskPriorityService _TaskPriorityService;

        public TaskPriorityController(IAccountService accountService,
                                   ITaskPriorityService TaskPriorityService) : base(accountService)
        {
            _TaskPriorityService = TaskPriorityService;
        }

        /// <summary>
        ///     Lấy tất cả trạng thái Task Priority
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_priority/all")]
        public async Task<ActionResult<IEnumerable<TaskPrioritySimpleModel>>> GetAll()
        {
            return Ok(await _TaskPriorityService.GetAll(await GetUserId()));
        }
        /// <summary>
        ///     Lấy tất cả trạng thái Task  Priority/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_priority")]
        public async Task<ActionResult<GetTaskPriorityPagingResponse>> GetPaging([FromQuery] GetTaskPriorityPagingRequest request)
        {
            return Ok(await _TaskPriorityService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy trạng thái Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_priority/{id}")]
        public async Task<ActionResult<TaskPrioritySimpleModel>> GetById(int id)
        {
            return Ok(await _TaskPriorityService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert trạng thái Task Priority
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/task_priority")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertTaskPriority request)
        {
            return Ok(await _TaskPriorityService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert trạng thái Task Priority
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/task_priority")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateTaskPriority request)
        {
            return Ok(await _TaskPriorityService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete trạng thái Task Priority
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/task_priority/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _TaskPriorityService.Delete(await GetUserId(), id));
        }
    }
}

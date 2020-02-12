using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.TaskServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.TaskStatus;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class TaskStatusController : BaseController
    {
        private readonly ITaskStatusService _TaskStatusService;

        public TaskStatusController(IAccountService accountService,
                                   ITaskStatusService TaskStatusService) : base(accountService)
        {
            _TaskStatusService = TaskStatusService;
        }

        /// <summary>
        ///     Lấy tất cả trạng thái Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_status/all")]
        public async Task<ActionResult<IEnumerable<TaskStatusSimpleModel>>> GetAll()
        {
            return Ok(await _TaskStatusService.GetAll(await GetUserId()));
        }
        /// <summary>
        ///     Lấy tất cả trạng thái Task/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_status")]
        public async Task<ActionResult<GetTaskStatusPagingResponse>> GetPaging([FromQuery] GetTaskStatusPagingRequest request)
        {
            return Ok(await _TaskStatusService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy trạng thái Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_status/{id}")]
        public async Task<ActionResult<TaskStatusSimpleModel>> GetById(int id)
        {
            return Ok(await _TaskStatusService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert trạng thái Task
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/task_status")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertTaskStatus request)
        {
            return Ok(await _TaskStatusService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert trạng thái Task
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/task_status")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateTaskStatus request)
        {
            return Ok(await _TaskStatusService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete trạng thái Task
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/task_status/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _TaskStatusService.Delete(await GetUserId(), id));
        }
    }
}

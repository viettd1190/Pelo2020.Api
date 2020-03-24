using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.TaskServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.TaskLoop;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class TaskLoopController : BaseController
    {
        private readonly ITaskLoopService _TaskLoopService;

        public TaskLoopController(IAccountService accountService,
                                   ITaskLoopService TaskLoopService) : base(accountService)
        {
            _TaskLoopService = TaskLoopService;
        }

        /// <summary>
        ///     Lấy tất cả Loop Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_loop/all")]
        public async Task<ActionResult<IEnumerable<TaskLoopSimpleModel>>> GetAll()
        {
            return Ok(await _TaskLoopService.GetAll(await GetUserId()));
        }
        /// <summary>
        ///     Lấy tất cả Loop Task/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_loop")]
        public async Task<ActionResult<GetTaskLoopPagingResponse>> GetPaging([FromQuery] GetTaskLoopPagingRequest request)
        {
            return Ok(await _TaskLoopService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy Loop Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task_loop/{id}")]
        public async Task<ActionResult<TaskLoopSimpleModel>> GetById(int id)
        {
            return Ok(await _TaskLoopService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert Loop Task
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/task_loop")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertTaskLoop request)
        {
            return Ok(await _TaskLoopService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert Loop Task
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/task_loop")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateTaskLoop request)
        {
            return Ok(await _TaskLoopService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete Loop Task
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/task_loop/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _TaskLoopService.Delete(await GetUserId(), id));
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.TaskServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Task;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class TaskController : BaseController
    {
        private readonly ITaskService _TaskService;

        public TaskController(IAccountService accountService,
                                   ITaskService TaskService) : base(accountService)
        {
            _TaskService = TaskService;
        }

        ///// <summary>
        /////     Lấy tất cả  Task
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/task/all")]
        //public async Task<ActionResult<IEnumerable<TaskSimpleModel>>> GetAll()
        //{
        //    return Ok(await _TaskService.GetAll(await GetUserId()));
        //}
        /// <summary>
        ///     Lấy tất cả  Task/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task")]
        public async Task<ActionResult<GetTaskPagingResponse>> GetPaging([FromQuery] GetTaskPagingRequest request)
        {
            return Ok(await _TaskService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy  Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/task/{id}")]
        public async Task<ActionResult<TaskSimpleModel>> GetById(int id)
        {
            return Ok(await _TaskService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert  Task
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/task")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertTask request)
        {
            return Ok(await _TaskService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert  Task
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/task")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateTask request)
        {
            return Ok(await _TaskService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete  Task
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/task/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _TaskService.Delete(await GetUserId(), id));
        }
    }
}

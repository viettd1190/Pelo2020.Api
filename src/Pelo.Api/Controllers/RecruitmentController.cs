using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.TaskServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Recruitment;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class RecruitmentController : BaseController
    {
        private readonly IRecruitmentService _RecruitmentService;

        public RecruitmentController(IAccountService accountService,
                                   IRecruitmentService RecruitmentService) : base(accountService)
        {
            _RecruitmentService = RecruitmentService;
        }

        /// <summary>
        ///     Lấy tất cả tuyển dụng Task/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/recruitment_")]
        public async Task<ActionResult<GetRecruitmentPagingResponse>> GetPaging([FromQuery] GetRecruitmentPagingRequest request)
        {
            return Ok(await _RecruitmentService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/recruitment_/{id}")]
        public async Task<ActionResult<RecruitmentSimpleModel>> GetById(int id)
        {
            return Ok(await _RecruitmentService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/recruitment_")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertRecruitment request)
        {
            return Ok(await _RecruitmentService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/recruitment_")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateRecruitment request)
        {
            return Ok(await _RecruitmentService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/recruitment_/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _RecruitmentService.Delete(await GetUserId(), id));
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.TaskServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.RecruitmentStatus;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class RecruitmentStatusController : BaseController
    {
        private readonly IRecruitmentStatusService _RecruitmentStatusService;

        public RecruitmentStatusController(IAccountService accountService,
                                   IRecruitmentStatusService RecruitmentStatusService) : base(accountService)
        {
            _RecruitmentStatusService = RecruitmentStatusService;
        }

        /// <summary>
        ///     Lấy tất cả tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/recruitment_status/all")]
        public async Task<ActionResult<IEnumerable<RecruitmentStatusSimpleModel>>> GetAll()
        {
            return Ok(await _RecruitmentStatusService.GetAll(await GetUserId()));
        }
        /// <summary>
        ///     Lấy tất cả tuyển dụng Task/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/recruitment_status")]
        public async Task<ActionResult<GetRecruitmentStatusPagingResponse>> GetPaging([FromQuery] GetRecruitmentStatusPagingRequest request)
        {
            return Ok(await _RecruitmentStatusService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/recruitment_status/{id}")]
        public async Task<ActionResult<RecruitmentStatusSimpleModel>> GetById(int id)
        {
            return Ok(await _RecruitmentStatusService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/recruitment_status")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertRecruitmentStatus request)
        {
            return Ok(await _RecruitmentStatusService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/recruitment_status")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateRecruitmentStatus request)
        {
            return Ok(await _RecruitmentStatusService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/recruitment_status/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _RecruitmentStatusService.Delete(await GetUserId(), id));
        }
    }
}

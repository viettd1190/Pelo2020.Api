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
        private readonly IRecruitmentService _recruitmentService;

        public RecruitmentController(IAccountService accountService,
                                     IRecruitmentService recruitmentService) : base(accountService)
        {
            _recruitmentService = recruitmentService;
        }

        /// <summary>
        ///     Lấy tất cả tuyển dụng Task/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/recruitment")]
        public async Task<ActionResult<GetRecruitmentPagingResponse>> GetPaging([FromQuery] GetRecruitmentPagingRequest request)
        {
            return Ok(await _recruitmentService.GetPaging(await GetUserId(),
                                                          request));
        }

        /// <summary>
        ///     Lấy tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/recruitment/{id}")]
        public async Task<ActionResult<RecruitmentSimpleModel>> GetById(int id)
        {
            return Ok(await _recruitmentService.GetById(await GetUserId(),
                                                        id));
        }

        /// <summary>
        ///     insert tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/recruitment")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertRecruitment request)
        {
            return Ok(await _recruitmentService.Insert(await GetUserId(),
                                                       request));
        }

        /// <summary>
        ///     insert tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/recruitment")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateRecruitment request)
        {
            return Ok(await _recruitmentService.Update(await GetUserId(),
                                                       request));
        }

        /// <summary>
        ///     delete tuyển dụng Task
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/recruitment/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _recruitmentService.Delete(await GetUserId(),
                                                       id));
        }
    }
}

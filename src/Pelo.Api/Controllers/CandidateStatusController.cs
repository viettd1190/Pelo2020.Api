using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CrmServices;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CrmPriority;
using Pelo.Common.Dtos.CandidateStatus;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CandidateStatusController : BaseController
    {
        private readonly ICandidateStatusService _CandidateStatusService;

        public CandidateStatusController(IAccountService accountService,
                                   ICandidateStatusService CandidateStatusService) : base(accountService)
        {
            _CandidateStatusService = CandidateStatusService;
        }

        /// <summary>
        ///     Lấy tất cả trạng thái Candidate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/candidate_status/all")]
        public async Task<ActionResult<IEnumerable<CandidateStatusSimpleModel>>> GetAll()
        {
            return Ok(await _CandidateStatusService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy tất cả trạng thái Candidate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/candidate_status")]
        public async Task<ActionResult<PageResult<GetCandidateStatusPagingResponse>>> GetPaging([FromQuery] GetCandidateStatusPagingRequest request)
        {
            return Ok(await _CandidateStatusService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy trạng thái Candidate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/candidate_status/{id}")]
        public async Task<ActionResult<GetCandidateStatusResponse>> GetById(int id)
        {
            return Ok(await _CandidateStatusService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert trạng thái Candidate
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/candidate_status")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertCandidateStatus request)
        {
            return Ok(await _CandidateStatusService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update trạng thái Candidate
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/candidate_status")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateCandidateStatus request)
        {
            return Ok(await _CandidateStatusService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete trạng thái Candidate
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/candidate_status/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _CandidateStatusService.Delete(await GetUserId(), id));
        }
    }
}

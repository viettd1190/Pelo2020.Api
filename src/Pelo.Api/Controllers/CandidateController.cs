using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CrmServices;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CrmPriority;
using Pelo.Common.Dtos.Candidate;
using Pelo.Common.Models;
using Pelo.Api.Services.CandidateServices;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CandidateController : BaseController
    {
        private readonly ICandidateService _CandidateService;

        public CandidateController(IAccountService accountService,
                                   ICandidateService CandidateService) : base(accountService)
        {
            _CandidateService = CandidateService;
        }

        /// <summary>
        ///     Lấy tất cả trạng thái Candidate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/candidate")]
        public async Task<ActionResult<PageResult<GetCandidatePagingResponse>>> GetPaging([FromQuery] GetCandidatePagingRequest request)
        {
            return Ok(await _CandidateService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy trạng thái Candidate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/candidate/{id}")]
        public async Task<ActionResult<GetCandidateResponse>> GetById(int id)
        {
            return Ok(await _CandidateService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert trạng thái Candidate
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/candidate")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertCandidate request)
        {
            return Ok(await _CandidateService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update trạng thái Candidate
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/candidate")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateCandidate request)
        {
            return Ok(await _CandidateService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete trạng thái Candidate
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/candidate/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _CandidateService.Delete(await GetUserId(), id));
        }
    }
}

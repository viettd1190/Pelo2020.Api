using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CrmServices;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CrmPriority;
using Pelo.Common.Dtos.CrmStatus;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CrmStatusController : BaseController
    {
        private readonly ICrmStatusService _crmStatusService;

        public CrmStatusController(IAccountService accountService,
                                   ICrmStatusService crmStatusService) : base(accountService)
        {
            _crmStatusService = crmStatusService;
        }

        /// <summary>
        ///     Lấy tất cả trạng thái CRM
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm_status/all")]
        public async Task<ActionResult<IEnumerable<CrmStatusSimpleModel>>> GetAll()
        {
            return Ok(await _crmStatusService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy tất cả trạng thái CRM
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm_status")]
        public async Task<ActionResult<PageResult<GetCrmStatusPagingResponse>>> GetPaging([FromQuery] GetCrmStatusPagingRequest request)
        {
            return Ok(await _crmStatusService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy trạng thái CRM
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm_status/{id}")]
        public async Task<ActionResult<GetCrmStatusResponse>> GetById(int id)
        {
            return Ok(await _crmStatusService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert trạng thái CRM
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/crm_status/insert")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertCrmStatus request)
        {
            return Ok(await _crmStatusService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update trạng thái CRM
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/crm_status/update")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateCrmStatus request)
        {
            return Ok(await _crmStatusService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete trạng thái CRM
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/crm_status/delete/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _crmStatusService.Delete(await GetUserId(), id));
        }
    }
}

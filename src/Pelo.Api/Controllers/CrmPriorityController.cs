using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CrmServices;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CrmPriority;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CrmPriorityController : BaseController
    {
        private readonly ICrmPriorityService _crmPriorityService;

        public CrmPriorityController(IAccountService accountService,
                                   ICrmPriorityService crmPriorityService) : base(accountService)
        {
            _crmPriorityService = crmPriorityService;
        }

        /// <summary>
        ///     Lấy tất cả mức độ khẩn cấp CRM
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm_priority/all")]
        public async Task<ActionResult<IEnumerable<CrmPrioritySimpleModel>>> GetAll()
        {
            return Ok(await _crmPriorityService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy tất cả mức độ khẩn cấp CRM/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm_priority")]
        public async Task<ActionResult<GetCrmPriorityPagingResponse>> GetPaging([FromQuery] GetCrmPriorityPagingRequest request)
        {
            return Ok(await _crmPriorityService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy mức độ khẩn cấp CRM
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm_priority/{id}")]
        public async Task<ActionResult<GetCrmPriorityResponse>> GetById(int id)
        {
            return Ok(await _crmPriorityService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert mức độ khẩn cấp CRM
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/crm_priority/insert")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertCrmPriority request)
        {
            return Ok(await _crmPriorityService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert mức độ khẩn cấp CRM
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/crm_priority/update")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateCrmPriority request)
        {
            return Ok(await _crmPriorityService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete mức độ khẩn cấp CRM
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/crm_priority/delete/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _crmPriorityService.Delete(await GetUserId(), id));
        }
    }
}

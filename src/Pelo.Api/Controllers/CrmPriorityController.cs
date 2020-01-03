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
    }
}

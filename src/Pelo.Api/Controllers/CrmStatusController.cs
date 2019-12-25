using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CrmStatus;

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
    }
}

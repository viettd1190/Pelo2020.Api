using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CrmType;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CrmTypeController : BaseController
    {
        private readonly ICrmTypeService _crmTypeService;

        public CrmTypeController(IAccountService accountService,
                                 ICrmTypeService crmTypeService) : base(accountService)
        {
            _crmTypeService = crmTypeService;
        }

        /// <summary>
        ///     Lấy tất cả kiểu chốt CRM
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm_type/all")]
        public async Task<ActionResult<IEnumerable<CrmTypeSimpleModel>>> GetAll()
        {
            return Ok(await _crmTypeService.GetAll(await GetUserId()));
        }
    }
}

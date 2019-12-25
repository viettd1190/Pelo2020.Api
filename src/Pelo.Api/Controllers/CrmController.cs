using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CrmServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Crm;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CrmController : BaseController
    {
        private readonly ICrmService _crmService;

        public CrmController(IAccountService accountService,
                             ICrmService crmService) : base(accountService)
        {
            _crmService = crmService;
        }

        /// <summary>
        ///     Lấy danh sách CRM có phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm")]
        public async Task<ActionResult<GetCrmPagingResponse>> GetByPaging([FromQuery] GetCrmPagingRequest request)
        {
            return Ok(await _crmService.GetPaging(await GetUserId(),
                                                  request));
        }
    }
}

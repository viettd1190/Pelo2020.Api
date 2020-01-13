using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CrmServices;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CrmPriority;
using Pelo.Common.Dtos.CrmType;
using Pelo.Common.Models;

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


        /// <summary>
        ///     Lấy tất cả kiểu chốt CRM
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm_status")]
        public async Task<ActionResult<PageResult<GetCrmTypePagingResponse>>> GetPaging([FromQuery] GetCrmTypePagingRequest request)
        {
            return Ok(await _crmTypeService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy kiểu chốt CRM
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm_status/{id}")]
        public async Task<ActionResult<GetCrmTypeResponse>> GetById(int id)
        {
            return Ok(await _crmTypeService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert kiểu chốt CRM
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/crm_status/insert")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertCrmType request)
        {
            return Ok(await _crmTypeService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update kiểu chốt CRM
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/crm_status/update")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateCrmType request)
        {
            return Ok(await _crmTypeService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete kiểu chốt CRM
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/crm_status/delete/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _crmTypeService.Delete(await GetUserId(), id));
        }
    }
}

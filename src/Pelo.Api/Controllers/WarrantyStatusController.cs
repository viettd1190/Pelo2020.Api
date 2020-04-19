using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.UserServices;
using Pelo.Api.Services.WarrantyServices;
using Pelo.Common.Dtos.Warranty;
using Pelo.Common.Dtos.WarrantyStatus;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class WarrantyStatusController : BaseController
    {
        private readonly IWarrantyStatusService _warrantyService;

        public WarrantyStatusController(IAccountService accountService, IWarrantyStatusService warranty) : base(accountService)
        {
            _warrantyService = warranty;
        }
        /// <summary>
        ///     Lấy tất cả trạng thái bảo hành
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("api/warranty_status/all")]
        public async Task<ActionResult<IEnumerable<WarrantyStatusSimpleModel>>> GetAll()
        {
            return Ok(await _warrantyService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy tất cả trạng thái bảo hành
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/warranty_status")]
        public async Task<ActionResult<PageResult<GetWarrantyStatusPagingResponse>>> GetPaging([FromQuery] GetWarrantyStatusPagingRequest request)
        {
            return Ok(await _warrantyService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy trạng thái bảo hành
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/warranty_status/{id}")]
        public async Task<ActionResult<GetWarrantyStatusResponse>> GetById(int id)
        {
            return Ok(await _warrantyService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert trạng thái bảo hành
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warranty_status")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertWarrantyStatus request)
        {
            return Ok(await _warrantyService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update trạng thái bảo hành
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/warranty_status")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateWarrantyStatus request)
        {
            return Ok(await _warrantyService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete trạng thái bảo hành
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/warranty_status/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _warrantyService.Delete(await GetUserId(), id));
        }
    }
}
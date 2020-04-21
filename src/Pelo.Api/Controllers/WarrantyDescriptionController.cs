using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.UserServices;
using Pelo.Api.Services.WarrantyServices;
using Pelo.Common.Dtos.Warranty;
using Pelo.Common.Dtos.WarrantyDescription;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class WarrantyDescriptionController : BaseController
    {
        private readonly IWarrantyDescriptionService _warrantyService;

        public WarrantyDescriptionController(IAccountService accountService, IWarrantyDescriptionService warranty) : base(accountService)
        {
            _warrantyService = warranty;
        }
        /// <summary>
        ///     Lấy tất cả trạng thái bảo hành
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("api/warranty_description/all")]
        public async Task<ActionResult<IEnumerable<WarrantyDescriptionSimpleModel>>> GetAll()
        {
            return Ok(await _warrantyService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy tất cả trạng thái bảo hành
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/warranty_description")]
        public async Task<ActionResult<PageResult<GetWarrantyDescriptionPagingResponse>>> GetPaging([FromQuery] GetWarrantyDescriptionPagingRequest request)
        {
            return Ok(await _warrantyService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy trạng thái bảo hành
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/warranty_description/{id}")]
        public async Task<ActionResult<GetWarrantyDescriptionPagingResponse>> GetById(int id)
        {
            return Ok(await _warrantyService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert trạng thái bảo hành
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warranty_description")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertWarrantyDescription request)
        {
            return Ok(await _warrantyService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update trạng thái bảo hành
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/warranty_description")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateWarrantyDescription request)
        {
            return Ok(await _warrantyService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete trạng thái bảo hành
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/warranty_description/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _warrantyService.Delete(await GetUserId(), id));
        }
    }
}
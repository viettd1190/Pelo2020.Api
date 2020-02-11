using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.InvoiceServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Manufacturer;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class ManufacturerControlle : BaseController
    {
        private readonly IManufacturerService _ManufacturerService;

        public ManufacturerControlle(IAccountService accountService,
                                   IManufacturerService ManufacturerService) : base(accountService)
        {
            _ManufacturerService = ManufacturerService;
        }

        /// <summary>
        ///     Lấy tất cả hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/manufacturer/all")]
        public async Task<ActionResult<IEnumerable<ManufacturerSimpleModel>>> GetAll()
        {
            return Ok(await _ManufacturerService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy tất cả hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/manufacturer")]
        public async Task<ActionResult<PageResult<GetManufacturerPagingResponse>>> GetPaging([FromQuery] GetManufacturerPagingRequest request)
        {
            return Ok(await _ManufacturerService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/manufacturer/{id}")]
        public async Task<ActionResult<GetManufacturerResponse>> GetById(int id)
        {
            return Ok(await _ManufacturerService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/manufacturer")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertManufacturerRequest request)
        {
            return Ok(await _ManufacturerService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/manufacturer")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateManufacturerRequest request)
        {
            return Ok(await _ManufacturerService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/manufacturer/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _ManufacturerService.Delete(await GetUserId(), id));
        }
    }
}

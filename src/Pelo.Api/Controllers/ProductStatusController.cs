using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.ProductStatus;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class ProductStatusController : BaseController
    {
        private readonly IProductStatusService _productStatusService;

        public ProductStatusController(IAccountService accountService,
                                     IProductStatusService productStatusService) : base(accountService)
        {
            _productStatusService = productStatusService;
        }

        /// <summary>
        ///     Lấy tất cả đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product_status/all")]
        public async Task<ActionResult<IEnumerable<ProductStatusSimpleModel>>> GetAll()
        {
            return Ok(await _productStatusService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy tất cả đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product_status")]
        public async Task<ActionResult<PageResult<GetProductStatusPagingResponse>>> GetPaging([FromQuery] GetProductStatusPagingRequest request)
        {
            return Ok(await _productStatusService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product_status/{id}")]
        public async Task<ActionResult<GetProductStatusReponse>> GetById(int id)
        {
            return Ok(await _productStatusService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/product_status")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertProductStatus request)
        {
            return Ok(await _productStatusService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/product_status")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateProductStatus request)
        {
            return Ok(await _productStatusService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/product_status/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _productStatusService.Delete(await GetUserId(), id));
        }
    }
}

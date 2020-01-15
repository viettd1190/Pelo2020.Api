using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.ProductUnit;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class ProductUnitController : BaseController
    {
        private readonly IProductUnitService _productUnitService;

        public ProductUnitController(IAccountService accountService,
                                     IProductUnitService productUnitService) : base(accountService)
        {
            _productUnitService = productUnitService;
        }

        /// <summary>
        ///     Lấy tất cả đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product_unit/all")]
        public async Task<ActionResult<IEnumerable<ProductUnitSimpleModel>>> GetAll()
        {
            return Ok(await _productUnitService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy tất cả đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product_unit")]
        public async Task<ActionResult<PageResult<GetProductUnitPagingResponse>>> GetPaging([FromQuery] GetProductUnitPagingRequest request)
        {
            return Ok(await _productUnitService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product_unit/{id}")]
        public async Task<ActionResult<GetProductUnitReponse>> GetById(int id)
        {
            return Ok(await _productUnitService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/product_unit")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertProductUnit request)
        {
            return Ok(await _productUnitService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/product_unit")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateProductUnit request)
        {
            return Ok(await _productUnitService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/product_unit/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _productUnitService.Delete(await GetUserId(), id));
        }
    }
}

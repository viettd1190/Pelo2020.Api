using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Product;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IAccountService accountService,
                                 IProductService productService) : base(accountService)
        {
            _productService = productService;
        }

        /// <summary>
        ///     Lấy tất cả sản phẩm 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product/all")]
        public async Task<ActionResult<IEnumerable<ProductSimpleModel>>> GetAll()
        {
            return Ok(await _productService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy tất cả sản phẩm /page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product")]
        public async Task<ActionResult<GetProductPagingResponse>> GetPaging([FromQuery] GetProductPagingRequest request)
        {
            return Ok(await _productService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy sản phẩm 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product/{id}")]
        public async Task<ActionResult<ProductModel>> GetById(int id)
        {
            return Ok(await _productService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert sản phẩm 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/product")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertProduct request)
        {
            return Ok(await _productService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert sản phẩm 
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/product")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateProduct request)
        {
            return Ok(await _productService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete sản phẩm 
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/product/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _productService.Delete(await GetUserId(), id));
        }
    }
}

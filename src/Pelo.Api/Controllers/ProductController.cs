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
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.ProductGroup;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class ProductGroupController : BaseController
    {
        private readonly IProductGroupService _productGroupService;

        public ProductGroupController(IAccountService accountService,
                                      IProductGroupService productGroupService) : base(accountService)
        {
            _productGroupService = productGroupService;
        }

        /// <summary>
        ///     Lấy tất cả nhóm sản phẩm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product_group/all")]
        public async Task<ActionResult<IEnumerable<ProductGroupSimpleModel>>> GetAll()
        {
            return Ok(await _productGroupService.GetAll(await GetUserId()));
        }
    }
}

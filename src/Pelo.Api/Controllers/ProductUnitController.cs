using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.ProductUnit;

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
    }
}

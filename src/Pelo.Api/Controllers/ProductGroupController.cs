using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.ProductGroup;
using Pelo.Common.Models;

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

        /// <summary>
        ///     Lấy tất cả nhóm sản phẩm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product_group")]
        public async Task<ActionResult<PageResult<GetProductGroupPagingResponse>>> GetPaging([FromQuery] GetProductGroupPagingRequest request)
        {
            return Ok(await _productGroupService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy nhóm sản phẩm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product_group/{id}")]
        public async Task<ActionResult<GetProductGroupReponse>> GetById(int id)
        {
            return Ok(await _productGroupService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert nhóm sản phẩm
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/product_group")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertProductGroup request)
        {
            return Ok(await _productGroupService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update nhóm sản phẩm
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/product_group")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateProductGroup request)
        {
            return Ok(await _productGroupService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete nhóm sản phẩm
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/product_group/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _productGroupService.Delete(await GetUserId(), id));
        }
    }
}

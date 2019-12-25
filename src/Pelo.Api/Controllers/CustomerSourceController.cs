using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CustomerSource;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CustomerSourceController : BaseController
    {
        private readonly ICustomerSourceService _customerSourceService;

        public CustomerSourceController(IAccountService accountService,
                                        ICustomerSourceService customerSourceService) : base(accountService)
        {
            _customerSourceService = customerSourceService;
        }

        /// <summary>
        ///     Lấy tất cả nhóm sản phẩm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer_source/all")]
        public async Task<ActionResult<IEnumerable<CustomerSourceSimpleModel>>> GetAll()
        {
            return Ok(await _customerSourceService.GetAll(await GetUserId()));
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.InvoiceServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.PayMethod;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class PayMethodController : BaseController
    {
        private readonly IPayMethodService _payMethodService;

        public PayMethodController(IAccountService accountService,
                                   IPayMethodService payMethodService) : base(accountService)
        {
            _payMethodService = payMethodService;
        }

        /// <summary>
        ///     Lấy tất cả hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/pay_method/all")]
        public async Task<ActionResult<IEnumerable<PayMethodSimpleModel>>> GetAll()
        {
            return Ok(await _payMethodService.GetAll(await GetUserId()));
        }


    }
}

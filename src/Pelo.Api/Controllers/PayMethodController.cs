using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.InvoiceServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.PayMethod;
using Pelo.Common.Models;

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

        /// <summary>
        ///     Lấy tất cả hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/pay_method")]
        public async Task<ActionResult<PageResult<GetPayMethodPagingResponse>>> GetPaging([FromQuery] GetPayMethodPagingRequest request)
        {
            return Ok(await _payMethodService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/pay_method/{id}")]
        public async Task<ActionResult<PayMethodModel>> GetById(int id)
        {
            return Ok(await _payMethodService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/pay_method")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertPayMethod request)
        {
            return Ok(await _payMethodService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/pay_method")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdatePayMethod request)
        {
            return Ok(await _payMethodService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/pay_method/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _payMethodService.Delete(await GetUserId(), id));
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.InvoiceServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.InvoiceStatus;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class InvoiceStatusController : BaseController
    {
        private readonly IInvoiceStatusService _invoiceStatusService;

        public InvoiceStatusController(IAccountService accountService,
                                   IInvoiceStatusService invoiceStatusService) : base(accountService)
        {
            _invoiceStatusService = invoiceStatusService;
        }

        /// <summary>
        ///     Lấy tất cả trạng thái đơn hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/invoice_status/all")]
        public async Task<ActionResult<IEnumerable<InvoiceStatusSimpleModel>>> GetAll()
        {
            return Ok(await _invoiceStatusService.GetAll(await GetUserId()));
        }
        /// <summary>
        ///     Lấy tất cả trạng thái đơn hàng/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/invoice_status")]
        public async Task<ActionResult<GetInvoiceStatusPagingResponse>> GetPaging([FromQuery] GetInvoiceStatusPagingRequest request)
        {
            return Ok(await _invoiceStatusService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy trạng thái đơn hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/invoice_status/{id}")]
        public async Task<ActionResult<GetInvoiceStatusResponse>> GetById(int id)
        {
            return Ok(await _invoiceStatusService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert trạng thái đơn hàng
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/invoice_status/insert")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertInvoiceStatus request)
        {
            return Ok(await _invoiceStatusService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert trạng thái đơn hàng
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/invoice_status/update")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateInvoiceStatus request)
        {
            return Ok(await _invoiceStatusService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete trạng thái đơn hàng
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/invoice_status/delete/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _invoiceStatusService.Delete(await GetUserId(), id));
        }
    }
}

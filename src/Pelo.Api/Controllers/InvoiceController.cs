using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.InvoiceServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Invoice;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class InvoiceController : BaseController
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IAccountService accountService,
                                 IInvoiceService invoiceService) : base(accountService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        [Route("api/invoice/get_by_customer")]
        public async Task<ActionResult<PageResult<GetInvoicePagingResponse>>> GetByCustomerId(int customerId,
                                                                                              int page,
                                                                                              int pageSize)
        {
            return Ok(await _invoiceService.GetByCustomerId(await GetUserId(),
                                                            customerId,
                                                            page,
                                                            pageSize));
        }

        /// <summary>
        ///     Lấy danh sách đơn hàng có phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/invoice")]
        public async Task<ActionResult<GetInvoicePagingResponse>> GetByPaging([FromQuery] GetInvoicePagingRequest request)
        {
            return Ok(await _invoiceService.GetPaging(await GetUserId(),
                                                      request));
        }
    }
}

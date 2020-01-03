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
    }
}

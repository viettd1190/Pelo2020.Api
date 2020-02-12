using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.ReceiptStatus;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class ReceiptStatusController : BaseController
    {
        private readonly IReceiptStatusService _ReceiptStatusService;

        public ReceiptStatusController(IAccountService accountService,
                                   IReceiptStatusService ReceiptStatusService) : base(accountService)
        {
            _ReceiptStatusService = ReceiptStatusService;
        }

        /// <summary>
        ///     Lấy tất cả trạng thái receipt
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/receipt_status/all")]
        public async Task<ActionResult<IEnumerable<ReceiptStatusSimpleModel>>> GetAll()
        {
            return Ok(await _ReceiptStatusService.GetAll(await GetUserId()));
        }
        /// <summary>
        ///     Lấy tất cả trạng thái receipt/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/receipt_status")]
        public async Task<ActionResult<GetReceiptStatusPagingResponse>> GetPaging([FromQuery] GetReceiptStatusPagingRequest request)
        {
            return Ok(await _ReceiptStatusService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy trạng thái receipt
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/receipt_status/{id}")]
        public async Task<ActionResult<ReceiptStatusSimpleModel>> GetById(int id)
        {
            return Ok(await _ReceiptStatusService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert trạng thái receipt
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receipt_status")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertReceiptStatus request)
        {
            return Ok(await _ReceiptStatusService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert trạng thái receipt
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/receipt_status")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateReceiptStatus request)
        {
            return Ok(await _ReceiptStatusService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete trạng thái receipt
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/receipt_status/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _ReceiptStatusService.Delete(await GetUserId(), id));
        }
    }
}

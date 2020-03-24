using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.ReceiptServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.ReceiptDescription;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class ReceiptDescriptionController : BaseController
    {
        private readonly IReceiptDescriptionService _ReceiptDescriptionService;

        public ReceiptDescriptionController(IAccountService accountService,
                                   IReceiptDescriptionService ReceiptDescriptionService) : base(accountService)
        {
            _ReceiptDescriptionService = ReceiptDescriptionService;
        }

        /// <summary>
        ///     Lấy tất cả trạng thái receipt description
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/receipt_description/all")]
        public async Task<ActionResult<IEnumerable<ReceiptDescriptionSimpleModel>>> GetAll()
        {
            return Ok(await _ReceiptDescriptionService.GetAll(await GetUserId()));
        }
        /// <summary>
        ///     Lấy tất cả trạng thái receipt  description/page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/receipt_description")]
        public async Task<ActionResult<GetReceiptDescriptionPagingResponse>> GetPaging([FromQuery] GetReceiptDescriptionPagingRequest request)
        {
            return Ok(await _ReceiptDescriptionService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy trạng thái receipt
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/receipt_description/{id}")]
        public async Task<ActionResult<GetReceiptDescriptionResponse>> GetById(int id)
        {
            return Ok(await _ReceiptDescriptionService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert trạng thái receipt description
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receipt_description")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertReceiptDescription request)
        {
            return Ok(await _ReceiptDescriptionService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     insert trạng thái receipt description
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/receipt_description")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateReceiptDescription request)
        {
            return Ok(await _ReceiptDescriptionService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete trạng thái receipt description
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/receipt_description/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _ReceiptDescriptionService.Delete(await GetUserId(), id));
        }
    }
}

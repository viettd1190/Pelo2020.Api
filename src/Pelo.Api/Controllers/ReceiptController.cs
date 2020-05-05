
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Receipt;
using Pelo.Common.Models;
using Pelo.Common.Extensions;
using Pelo.Api.Services.ReceiptServices;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class ReceiptController : BaseController
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IAccountService accountService,
                                 IReceiptService receiptService) : base(accountService)
        {
            _receiptService = receiptService;
        }
        [HttpGet]
        [Route("api/Receipt/get_by_customer")]
        public async Task<ActionResult<PageResult<GetReceiptPagingResponse>>> GetByCustomerId(int customerId,
                                                                                              int page,
                                                                                              int pageSize)
        {
            return Ok(await _receiptService.GetByCustomerId(await GetUserId(),
                                                            customerId,
                                                            page,
                                                            pageSize));
        }

        /// <summary>
        ///     Lấy danh sách  bao hanh có phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Receipt")]
        public async Task<ActionResult<GetReceiptPagingResponse>> GetByPaging([FromQuery] GetReceiptPagingRequest request)
        {
            return Ok(await _receiptService.GetPaging(await GetUserId(),
                                                      request));
        }

        /// <summary>
        ///     Thêm mới bao hanh
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Receipt")]
        public async Task<ActionResult<TResponse<bool>>> Insert(InsertReceiptRequest request)
        {
            return Ok(await _receiptService.Insert(await GetUserId(), request));
        }

        /// <summary>
        /// Lấy chi tiết bao hanh
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Receipt/{id}")]
        public async Task<ActionResult<TResponse<GetReceiptByIdResponse>>> GetById(int id)
        {
            return Ok(await _receiptService.GetById(await GetUserId(), id));
        }

        /// <summary>
        ///     Update CRM
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Receipt")]
        public async Task<ActionResult<bool>> UpdateCrm([FromBody] UpdateReceiptRequest request)
        {
            return Ok(await _receiptService.UpdateCrm(await GetUserId(),
                                                  request));
        }

        /// <summary>
        ///     Them CRM Comment
        /// </summary>
        /// <param name="paras"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Receipt/comment")]
        //public async Task<ActionResult<bool>> Comment([FromForm] CommentCrmRequest request)
        public async Task<ActionResult<bool>> Comment([FromForm] string paras, [FromForm] IFormFileCollection files)
        {
            //var parametes = paras.Split('&');
            //int id = Convert.ToInt32(parametes[0]
            //                                 .Replace("id=", ""));
            //string comment = parametes[1]
            //        .Replace("comment=", "");

            var parameters = WebUtility.UrlDecode(paras)
                                       .Replace("para=", "")
                                       .ToObject<Tuple<int, string>>();


            return Ok(await _receiptService.Comment(await GetUserId(),
                                                new CommentReceiptRequest
                                                {
                                                    Id = parameters.Item1,
                                                    Comment = parameters.Item2,
                                                    Files = files
                                                }));
        }

        /// <summary>
        ///     Lấy danh sách log crm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm/log/{id}")]
        public async Task<ActionResult<TResponse<IEnumerable<ReceiptLogResponse>>>> GetLog([FromRoute] int id)
        {
            return Ok(await _receiptService.GetLogs(await GetUserId(), id));
        }
    }   
    
}
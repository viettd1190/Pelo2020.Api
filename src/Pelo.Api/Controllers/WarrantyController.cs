using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.UserServices;
using Pelo.Api.Services.WarrantyServices;
using Pelo.Common.Dtos.Warranty;
using Pelo.Common.Models;
using Pelo.Common.Extensions;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class WarrantyController : BaseController
    {
        private readonly IWarrantyService _warrantySerivce;

        public WarrantyController(IAccountService accountService,
                                 IWarrantyService warrantySerivce) : base(accountService)
        {
            _warrantySerivce = warrantySerivce;
        }
        [HttpGet]
        [Route("api/warranty/get_by_customer")]
        public async Task<ActionResult<PageResult<GetWarrantyPagingResponse>>> GetByCustomerId(int customerId,
                                                                                              int page,
                                                                                              int pageSize)
        {
            return Ok(await _warrantySerivce.GetByCustomerId(await GetUserId(),
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
        [Route("api/warranty")]
        public async Task<ActionResult<GetWarrantyPagingResponse>> GetByPaging([FromQuery] GetWarrantyPagingRequest request)
        {
            return Ok(await _warrantySerivce.GetPaging(await GetUserId(),
                                                      request));
        }

        /// <summary>
        ///     Thêm mới bao hanh
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warranty")]
        public async Task<ActionResult<TResponse<bool>>> Insert(InsertWarrantyRequest request)
        {
            return Ok(await _warrantySerivce.Insert(await GetUserId(), request));
        }

        /// <summary>
        /// Lấy chi tiết bao hanh
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/warranty/{id}")]
        public async Task<ActionResult<TResponse<GetWarrantyByIdResponse>>> GetById(int id)
        {
            return Ok(await _warrantySerivce.GetById(await GetUserId(), id));
        }

        /// <summary>
        ///     Update bao hanh
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/warranty")]
        public async Task<ActionResult<bool>> UpdateCrm([FromBody] UpdateWarrantyRequest request)
        {
            return Ok(await _warrantySerivce.UpdateCrm(await GetUserId(),
                                                  request));
        }

        /// <summary>
        ///     Them bao hanh Comment
        /// </summary>
        /// <param name="paras"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warranty/comment")]
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


            return Ok(await _warrantySerivce.Comment(await GetUserId(),
                                                new CommentWarrantyRequest
                                                {
                                                    Id = parameters.Item1,
                                                    Comment = parameters.Item2,
                                                    Files = files
                                                }));
        }

        /// <summary>
        ///     Lấy danh sách log bao hanh
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/warranty/warranty_log/{id}")]
        public async Task<ActionResult<TResponse<IEnumerable<WarrantyLogResponse>>>> GetLog(int id)
        {
            return Ok(await _warrantySerivce.GetLogs(await GetUserId(), id));
        }
    }

}
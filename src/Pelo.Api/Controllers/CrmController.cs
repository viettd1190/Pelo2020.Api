﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CrmServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Crm;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CrmController : BaseController
    {
        private readonly ICrmService _crmService;

        public CrmController(IAccountService accountService,
                             ICrmService crmService) : base(accountService)
        {
            _crmService = crmService;
        }

        /// <summary>
        ///     Lấy danh sách CRM có phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm")]
        public async Task<ActionResult<PageResult<GetCrmPagingResponse>>> GetByPaging([FromQuery] GetCrmPagingRequest request)
        {
            return Ok(await _crmService.GetPaging(await GetUserId(),
                                                  request));
        }

        [HttpGet]
        [Route("api/crm/khach_chua_xu_ly_trong_ngay")]
        public async Task<ActionResult<PageResult<GetCrmPagingResponse>>> KhachChuaXuLyTrongNgay([FromQuery] GetPagingModel request)
        {
            return Ok(await _crmService.KhachChuaXuLyTrongNgay(await GetUserId(),
                                                               request));
        }

        [HttpGet]
        [Route("api/crm/khach_toi_hen_can_cham_soc")]
        public async Task<ActionResult<PageResult<GetCrmPagingResponse>>> KhachToiHenCanChamSoc([FromQuery] GetPagingModel request)
        {
            return Ok(await _crmService.KhachToiHenCanChamSoc(await GetUserId(),
                                                              request));
        }

        [HttpGet]
        [Route("api/crm/khach_qua_hen_cham_soc")]
        public async Task<ActionResult<PageResult<GetCrmPagingResponse>>> KhachQuaHenChamSoc([FromQuery] GetPagingModel request)
        {
            return Ok(await _crmService.KhachQuaHenChamSoc(await GetUserId(),
                                                           request));
        }

        [HttpGet]
        [Route("api/crm/khach_toi_hen_ngay_mai")]
        public async Task<ActionResult<PageResult<GetCrmPagingResponse>>> KhachToiHenNgayMai([FromQuery] GetPagingModel request)
        {
            return Ok(await _crmService.KhachToiHenNgayMai(await GetUserId(),
                                                           request));
        }

        /// <summary>
        ///     Them CRM
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/crm")]
        public async Task<ActionResult<bool>> InsertCrm([FromBody] InsertCrmRequest request)
        {
            return Ok(await _crmService.InsertCrm(await GetUserId(),
                                                  request));
        }

        [HttpGet]
        [Route("api/crm/get_by_customer")]
        public async Task<ActionResult<PageResult<GetCrmPagingResponse>>> GetByCustomerId(int customerId,
                                                                                          int page,
                                                                                          int pageSize)
        {
            return Ok(await _crmService.GetByCustomerId(await GetUserId(),
                                                        customerId,
                                                        page,
                                                        pageSize));
        }

        [HttpGet]
        [Route("api/crm/{id}")]
        public async Task<ActionResult<GetCrmModelReponse>> GetCrmById(int id)
        {
            return Ok(await _crmService.GetById(await GetUserId(),
                                                id));
        }

        /// <summary>
        ///     Update CRM
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/crm")]
        public async Task<ActionResult<bool>> UpdateCrm([FromBody] UpdateCrmRequest request)
        {
            return Ok(await _crmService.UpdateCrm(await GetUserId(),
                                                  request));
        }

        /// <summary>
        ///     Them CRM Comment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/crm/comment")]
        public async Task<ActionResult<bool>> Comment([FromForm] CommentCrmRequest request)
        {
            return Ok(await _crmService.UpdateComment(await GetUserId(),
                                                      request));
        }

        /// <summary>
        ///     Lấy danh sách log crm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/crm/log/{id}")]
        public async Task<ActionResult<TResponse<IEnumerable<CrmLogResponse>>>> GetLog([FromRoute] int id)
        {
            return Ok(await _crmService.GetCrmLogs(await GetUserId(), id));
        }
    }
}

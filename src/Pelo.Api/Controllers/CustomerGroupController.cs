using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CustomerGroup;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CustomerGroupController : BaseController
    {
        private readonly ICustomerGroupService _customerGroupService;

        public CustomerGroupController(IAccountService accountService,
                                       ICustomerGroupService customerGroupService) : base(accountService)
        {
            _customerGroupService = customerGroupService;
        }

        /// <summary>
        ///     Lấy tất cả nhóm khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer_group/all")]
        public async Task<ActionResult<IEnumerable<CustomerGroupSimpleModel>>> GetAll()
        {
            return Ok(await _customerGroupService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy danh sách nhóm khách hàng có phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer_group")]
        public async Task<ActionResult<PageResult<GetCustomerGroupPagingResponse>>> GetByPaging([FromQuery] GetCustomerGroupPagingRequest request)
        {
            return Ok(await _customerGroupService.GetPaging(await GetUserId(),
                                                            request));
        }

        /// <summary>
        ///     Thêm mới nhóm khách hàng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer_group")]
        public async Task<ActionResult<bool>> Insert(InsertCustomerGroupRequest request)
        {
            return Ok(await _customerGroupService.Insert(await GetUserId(),
                                                         request));
        }

        /// <summary>
        ///     Lấy thông tin nhóm khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer_group/{id}")]
        public async Task<ActionResult<GetCustomerGroupByIdResponse>> GetById(int id)
        {
            return Ok(await _customerGroupService.GetById(await GetUserId(),
                                                          id));
        }

        /// <summary>
        ///     Cập nhật thông tin nhóm khách hàng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/customer_group")]
        public async Task<ActionResult<bool>> Update(UpdateCustomerGroupRequest request)
        {
            return Ok(await _customerGroupService.Update(await GetUserId(),
                                                         request));
        }

        /// <summary>
        ///     Xóa nhóm khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/customer_group/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _customerGroupService.Delete(await GetUserId(),
                                                         id));
        }
    }
}

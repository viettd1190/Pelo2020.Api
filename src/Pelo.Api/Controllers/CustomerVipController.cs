using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CustomerVip;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CustomerVipController : BaseController
    {
        private readonly ICustomerVipService _customerVipService;

        public CustomerVipController(IAccountService accountService,
                                     ICustomerVipService customerVipService) : base(accountService)
        {
            _customerVipService = customerVipService;
        }

        /// <summary>
        ///     Lấy danh sách mức độ khách hàng thân thiết có phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer_vip")]
        public async Task<ActionResult<GetCustomerVipPagingResponse>> GetByPaging([FromQuery] GetCustomerVipPagingRequest request)
        {
            return Ok(await _customerVipService.GetPaging(await GetUserId(),
                                                          request));
        }

        /// <summary>
        ///     Thêm mới nhóm khách hàng thân thiết
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer_vip")]
        public async Task<ActionResult<bool>> Insert(InsertCustomerVipRequest request)
        {
            return Ok(await _customerVipService.Insert(await GetUserId(),
                                                       request));
        }

        /// <summary>
        ///     Lấy thông tin nhóm khách hàng thân thiết
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer_vip/{id}")]
        public async Task<ActionResult<GetCustomerVipByIdResponse>> GetById(int id)
        {
            return Ok(await _customerVipService.GetById(await GetUserId(),
                                                        id));
        }

        /// <summary>
        ///     Cập nhật thông tin nhóm khách hàng thân thiết
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/customer_vip")]
        public async Task<ActionResult<bool>> Update(UpdateCustomerVipRequest request)
        {
            return Ok(await _customerVipService.Update(await GetUserId(),
                                                       request));
        }

        /// <summary>
        ///     Xóa nhóm khách hàng thân thiết
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/customer_vip/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _customerVipService.Delete(await GetUserId(),
                                                       id));
        }
    }
}

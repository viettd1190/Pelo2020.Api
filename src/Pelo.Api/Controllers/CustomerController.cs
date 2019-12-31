using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Customer;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(IAccountService accountService,
                                  ICustomerService customerService) : base(accountService)
        {
            _customerService = customerService;
        }

        /// <summary>
        ///     Lấy danh sách khách hàng có phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer")]
        public async Task<ActionResult<GetCustomerPagingResponse>> GetByPaging([FromQuery] GetCustomerPagingRequest request)
        {
            return Ok(await _customerService.GetPaging(await GetUserId(),
                                                       request));
        }

        /// <summary>
        ///     Thêm mới khách hàng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer")]
        public async Task<ActionResult<bool>> Insert(InsertCustomerRequest request)
        {
            return Ok(await _customerService.Insert(await GetUserId(),
                                                    request));
        }

        /// <summary>
        ///     Lấy thông tin khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer/{id}")]
        public async Task<ActionResult<GetCustomerByIdResponse>> GetById(int id)
        {
            return Ok(await _customerService.GetById(await GetUserId(),
                                                     id));
        }

        /// <summary>
        ///     Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/customer")]
        public async Task<ActionResult<bool>> Update(UpdateCustomerRequest request)
        {
            return Ok(await _customerService.Update(await GetUserId(),
                                                    request));
        }

        /// <summary>
        ///     Xóa khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/customer/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _customerService.Delete(await GetUserId(),
                                                    id));
        }

        /// <summary>
        ///     Tìm kiếm khách hàng theo số điện thoại
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer/get_by_phone")]
        public async Task<ActionResult<CustomerByPhoneResponse>> GetByPhone(string phone)
        {
            return Ok(await _customerService.GetByPhone(await GetUserId(),
                                                        phone));
        }
    }
}

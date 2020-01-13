using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CustomerSource;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CustomerSourceController : BaseController
    {
        private readonly ICustomerSourceService _customerSourceService;

        public CustomerSourceController(IAccountService accountService,
                                        ICustomerSourceService customerSourceService) : base(accountService)
        {
            _customerSourceService = customerSourceService;
        }

        /// <summary>
        ///     Lấy tất cả nguồn khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer_source/all")]
        public async Task<ActionResult<IEnumerable<CustomerSourceSimpleModel>>> GetAll()
        {
            return Ok(await _customerSourceService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy danh sách nguồn khách hàng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer_source")]
        public async Task<ActionResult<PageResult<GetCustomerSourcePagingResponse>>> GetByPaging([FromQuery] GetCustomerSourcePagingRequest request)
        {
            return Ok(await _customerSourceService.GetPaging(await GetUserId(),
                                                            request));
        }

        /// <summary>
        ///     Thêm mới nguồn khách hàng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer_source")]
        public async Task<ActionResult<bool>> Insert(InsertCustomerSource request)
        {
            return Ok(await _customerSourceService.Insert(await GetUserId(),
                                                         request));
        }

        /// <summary>
        ///     Lấy thông tin nguồn khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customer_source/{id}")]
        public async Task<ActionResult<GetCustomerSourceResponse>> GetById(int id)
        {
            return Ok(await _customerSourceService.GetById(await GetUserId(),
                                                          id));
        }

        /// <summary>
        ///     Cập nhật thông tin nguồn khách hàng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/customer_source")]
        public async Task<ActionResult<bool>> Update(UpdateCustomerSource request)
        {
            return Ok(await _customerSourceService.Update(await GetUserId(),
                                                         request));
        }

        /// <summary>
        ///     Xóa nguồn khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/customer_source/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _customerSourceService.Delete(await GetUserId(),
                                                         id));
        }
    }
}

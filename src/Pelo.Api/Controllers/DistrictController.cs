using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.District;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class DistrictController : BaseController
    {
        private readonly IDistrictService _districtService;

        public DistrictController(IAccountService accountService,
                                  IDistrictService districtService) : base(accountService)
        {
            _districtService = districtService;
        }

        /// <summary>
        ///     Lấy tất cả quận huyện của tỉnh thành
        /// </summary>
        /// <param name="id">Id tỉnh thành</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/district/{id}/all")]
        public async Task<ActionResult<IEnumerable<DistrictModel>>> GetAll(int id)
        {
            return Ok(await _districtService.GetAll(id));
        }
        /// <summary>
        ///     Lấy tất cả quận huyện của tỉnh thành
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/district")]
        public async Task<ActionResult<PageResult<GetDistrictPagingResponse>>> GetPaging([FromQuery] GetDistrictPagingRequest request)
        {
            return Ok(await _districtService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy quận huyện của tỉnh thành
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/district/{id}")]
        public async Task<ActionResult<DistrictModel>> GetById(int id)
        {
            return Ok(await _districtService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert quận huyện của tỉnh thành
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/district")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertDistrict request)
        {
            return Ok(await _districtService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update quận huyện của tỉnh thành
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/district")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateDistrict request)
        {
            return Ok(await _districtService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete quận huyện của tỉnh thành
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/district/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _districtService.Delete(await GetUserId(), id));
        }
    }
}

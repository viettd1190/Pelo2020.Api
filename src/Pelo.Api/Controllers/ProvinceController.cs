using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Province;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class ProvinceController : BaseController
    {
        private readonly IProvinceService _provinceService;

        public ProvinceController(IAccountService accountService,
                                  IProvinceService provinceService) : base(accountService)
        {
            _provinceService = provinceService;
        }

        /// <summary>
        ///     Lấy tất cả tỉnh thành
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/province/all")]
        public async Task<ActionResult<IEnumerable<ProvinceModel>>> GetAll()
        {
            return Ok(await _provinceService.GetAll());
        }

        /// <summary>
        ///     Lấy tất cả province
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/province")]
        public async Task<ActionResult<PageResult<GetProvincePagingResponse>>> GetPaging([FromQuery] GetProvincePagingRequest request)
        {
            return Ok(await _provinceService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy province
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/province/{id}")]
        public async Task<ActionResult<ProvinceModel>> GetById(int id)
        {
            return Ok(await _provinceService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert province
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/province")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertProvince request)
        {
            return Ok(await _provinceService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update province
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/province")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateProvince request)
        {
            return Ok(await _provinceService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete province
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/province/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _provinceService.Delete(await GetUserId(), id));
        }
    }
}

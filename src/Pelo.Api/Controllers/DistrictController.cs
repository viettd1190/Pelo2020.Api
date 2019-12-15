using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.District;

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
    }
}

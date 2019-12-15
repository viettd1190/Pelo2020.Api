using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Province;

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
    }
}

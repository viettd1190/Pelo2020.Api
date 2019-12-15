using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Ward;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class WardController : BaseController
    {
        private readonly IWardService _wardService;

        public WardController(IAccountService accountService,
                                  IWardService wardService) : base(accountService)
        {
            _wardService = wardService;
        }

        /// <summary>
        ///     Lấy tất cả phường xã của quận huyện
        /// </summary>
        /// <param name="id">Id tỉnh thành</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ward/{id}/all")]
        public async Task<ActionResult<IEnumerable<WardModel>>> GetAll(int id)
        {
            return Ok(await _wardService.GetAll(id));
        }
    }
}

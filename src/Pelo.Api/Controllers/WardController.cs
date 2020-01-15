using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Ward;
using Pelo.Common.Models;

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

        /// <summary>
        ///     Lấy tất cả phường xã
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ward")]
        public async Task<ActionResult<PageResult<GetWardPagingResponse>>> GetPaging([FromQuery] GetWardPagingRequest request)
        {
            return Ok(await _wardService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy phường xã
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ward/{id}")]
        public async Task<ActionResult<WardModel>> GetById(int id)
        {
            return Ok(await _wardService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert phường xã
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ward")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertWard request)
        {
            return Ok(await _wardService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update phường xã
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/ward")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateWard request)
        {
            return Ok(await _wardService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete phường xã
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/ward/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _wardService.Delete(await GetUserId(), id));
        }
    }
}

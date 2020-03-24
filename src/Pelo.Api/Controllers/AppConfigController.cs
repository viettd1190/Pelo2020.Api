using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.AppConfig;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class AppConfigController : BaseController
    {
        private readonly IAppConfigService _appConfigService;

        public AppConfigController(IAccountService accountService,
                                   IAppConfigService appConfigService) : base(accountService)
        {
            _appConfigService = appConfigService;
        }

        /// <summary>
        ///     Lấy danh sách app config có phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/app_config")]
        public async Task<ActionResult<GetAppConfigPagingResponse>> GetByPaging([FromQuery] GetAppConfigPagingRequest request)
        {
            return Ok(await _appConfigService.GetPaging(await GetUserId(),
                                                        request));
        }

        /// <summary>
        ///     Thêm mới app config
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/app_config")]
        public async Task<ActionResult<bool>> Insert(InsertAppConfigRequest request)
        {
            return Ok(await _appConfigService.Insert(await GetUserId(),
                                                     request));
        }

        /// <summary>
        ///     Lấy thông tin app config
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/app_config/{id}")]
        public async Task<ActionResult<GetAppConfigByIdResponse>> GetById(int id)
        {
            return Ok(await _appConfigService.GetById(await GetUserId(),
                                                      id));
        }

        /// <summary>
        ///     Cập nhật thông tin app config
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/app_config")]
        public async Task<ActionResult<bool>> Update(UpdateAppConfigRequest request)
        {
            return Ok(await _appConfigService.Update(await GetUserId(),
                                                     request));
        }

        /// <summary>
        ///     Xóa app config
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/app_config/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _appConfigService.Delete(await GetUserId(),
                                                     id));
        }

        /// <summary>
        ///     Get value of app config by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/app_config/get_by_name")]
        public async Task<ActionResult<TResponse<string>>> GetByName(string name)
        {
            return Ok(await _appConfigService.GetByName(name));
        }
    }
}

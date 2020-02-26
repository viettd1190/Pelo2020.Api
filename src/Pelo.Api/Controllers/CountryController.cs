using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.CountryServices;
using Pelo.Api.Services.CrmServices;
using Pelo.Api.Services.CustomerServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Country;
using Pelo.Common.Dtos.CrmPriority;
using Pelo.Common.Dtos.CrmType;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class CountryController : BaseController
    {
        private readonly ICountryService _countryService;

        public CountryController(IAccountService accountService,
                                 ICountryService countryService) : base(accountService)
        {
            _countryService = countryService;
        }

        /// <summary>
        ///     Lấy tất cả quoc gia
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/country/all")]
        public async Task<ActionResult<IEnumerable<GetCountryReponse>>> GetAll()
        {
            return Ok(await _countryService.GetAll(await GetUserId()));
        }


        /// <summary>
        ///     Lấy tất cả quoc gia
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/country")]
        public async Task<ActionResult<PageResult<GetCountryPagingResponse>>> GetPaging([FromQuery] GetCountryPagingRequest request)
        {
            return Ok(await _countryService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy quoc gia
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/country/{id}")]
        public async Task<ActionResult<GetCountryPagingResponse>> GetById(int id)
        {
            return Ok(await _countryService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert quoc gia
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/country")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertCountry request)
        {
            return Ok(await _countryService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update quoc gia
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/country")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateCountry request)
        {
            return Ok(await _countryService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete quoc gia
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/country/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _countryService.Delete(await GetUserId(), id));
        }
    }
}

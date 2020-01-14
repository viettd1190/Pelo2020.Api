using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Branch;
using Pelo.Common.Models;

namespace Pelo.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    public class BranchController : BaseController
    {
        private readonly IBranchService _branchService;

        public BranchController(IAccountService accountService,
                                IBranchService branchService) : base(accountService)
        {
            _branchService = branchService;
        }

        /// <summary>
        ///     Lấy tất cả chi nhánh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/branch/all")]
        public async Task<ActionResult<IEnumerable<BranchSimpleModel>>> GetAll()
        {
            return Ok(await _branchService.GetAll(await GetUserId()));
        }

        /// <summary>
        ///     Lấy tất cả chi nhánh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/branch")]
        public async Task<ActionResult<PageResult<GetBranchPagingResponse>>> GetPaging([FromQuery] GetBranchPagingRequest request)
        {
            return Ok(await _branchService.GetPaging(await GetUserId(), request));
        }
        /// <summary>
        ///     Lấy chi nhánh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/branch/{id}")]
        public async Task<ActionResult<BranchModel>> GetById(int id)
        {
            return Ok(await _branchService.GetById(await GetUserId(), id));
        }
        /// <summary>
        ///     insert chi nhánh
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/branch")]
        public async Task<ActionResult<bool>> Insert([FromBody] InsertBranch request)
        {
            return Ok(await _branchService.Insert(await GetUserId(), request));
        }

        /// <summary>
        ///     update chi nhánh
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/branch")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateBranch request)
        {
            return Ok(await _branchService.Update(await GetUserId(), request));
        }
        /// <summary>
        ///     delete chi nhánh
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/branch/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return Ok(await _branchService.Delete(await GetUserId(), id));
        }
    }
}

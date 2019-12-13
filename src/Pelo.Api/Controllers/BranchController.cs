using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Branch;

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
    }
}

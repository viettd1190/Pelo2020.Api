using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Branch;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IBranchService
    {
        Task<TResponse<IEnumerable<BranchSimpleModel>>> GetAll(int userId);
    }

    public class BranchService : BaseService,
                                 IBranchService
    {
        private readonly IRoleService _roleService;

        public BranchService(IDapperReadOnlyRepository readOnlyRepository,
                             IDapperWriteRepository writeRepository,
                             IHttpContextAccessor context,
                             IRoleService roleService) : base(readOnlyRepository,
                                                              writeRepository,
                                                              context)
        {
            _roleService = roleService;
        }

        #region IBranchService Members

        public async Task<TResponse<IEnumerable<BranchSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<BranchSimpleModel>(SqlQuery.BRANCH_GET_ALL);
                    if(result.IsSuccess) return await Ok(result.Data);

                    return await Fail<IEnumerable<BranchSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<BranchSimpleModel>>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<BranchSimpleModel>>(exception);
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Department;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IDepartmentService
    {
        Task<TResponse<IEnumerable<DepartmentSimpleModel>>> GetAll(int userId);
    }

    public class DepartmentService : BaseService,
                                     IDepartmentService
    {
        private readonly IRoleService _roleService;

        public DepartmentService(IDapperReadOnlyRepository readOnlyRepository,
                                 IDapperWriteRepository writeRepository,
                                 IHttpContextAccessor context,
                                 IRoleService roleService) : base(readOnlyRepository,
                                                                  writeRepository,
                                                                  context)
        {
            _roleService = roleService;
        }

        #region IDepartmentService Members

        public async Task<TResponse<IEnumerable<DepartmentSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<DepartmentSimpleModel>(SqlQuery.DEPARTMENT_GET_ALL);
                    if(result.IsSuccess) return await Ok(result.Data);

                    return await Fail<IEnumerable<DepartmentSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<DepartmentSimpleModel>>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<DepartmentSimpleModel>>(exception);
            }
        }

        #endregion
    }
}

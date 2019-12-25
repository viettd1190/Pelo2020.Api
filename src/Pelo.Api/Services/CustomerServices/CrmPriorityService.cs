using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CrmPriority;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.CustomerServices
{
    public interface ICrmPriorityService
    {
        Task<TResponse<IEnumerable<CrmPrioritySimpleModel>>> GetAll(int userId);
    }

    public class CrmPriorityService : BaseService,
                                      ICrmPriorityService
    {
        readonly IRoleService _roleService;

        public CrmPriorityService(IDapperReadOnlyRepository readOnlyRepository,
                                  IDapperWriteRepository writeRepository,
                                  IHttpContextAccessor context,
                                  IRoleService roleService) : base(readOnlyRepository,
                                                                   writeRepository,
                                                                   context)
        {
            _roleService = roleService;
        }

        #region ICrmPriorityService Members

        public async Task<TResponse<IEnumerable<CrmPrioritySimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if(canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<CrmPrioritySimpleModel>(SqlQuery.CRM_PRIORITY_GET_ALL);
                    if(result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<CrmPrioritySimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<CrmPrioritySimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<CrmPrioritySimpleModel>>(exception);
            }
        }

        #endregion

        private async Task<TResponse<bool>> CanGetAll(int userId)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    return await Ok(true);
                }

                return await Fail<bool>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }
    }
}

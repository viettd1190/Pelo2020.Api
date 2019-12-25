using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CustomerSource;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.CustomerServices
{
    public interface ICustomerSourceService
    {
        Task<TResponse<IEnumerable<CustomerSourceSimpleModel>>> GetAll(int userId);
    }

    public class CustomerSourceService : BaseService,
                                         ICustomerSourceService
    {
        readonly IRoleService _roleService;

        public CustomerSourceService(IDapperReadOnlyRepository readOnlyRepository,
                                     IDapperWriteRepository writeRepository,
                                     IHttpContextAccessor context,
                                     IRoleService roleService) : base(readOnlyRepository,
                                                                      writeRepository,
                                                                      context)
        {
            _roleService = roleService;
        }

        #region ICustomerSourceService Members

        public async Task<TResponse<IEnumerable<CustomerSourceSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if(canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<CustomerSourceSimpleModel>(SqlQuery.CUSTOMER_SOURCE_GET_ALL);
                    if(result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<CustomerSourceSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<CustomerSourceSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<CustomerSourceSimpleModel>>(exception);
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

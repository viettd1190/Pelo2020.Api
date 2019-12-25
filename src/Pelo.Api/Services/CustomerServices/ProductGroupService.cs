using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.ProductGroup;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.CustomerServices
{
    public interface IProductGroupService
    {
        Task<TResponse<IEnumerable<ProductGroupSimpleModel>>> GetAll(int userId);
    }

    public class ProductGroupService : BaseService,
                                       IProductGroupService
    {
        readonly IRoleService _roleService;

        public ProductGroupService(IDapperReadOnlyRepository readOnlyRepository,
                                   IDapperWriteRepository writeRepository,
                                   IHttpContextAccessor context,
                                   IRoleService roleService) : base(readOnlyRepository,
                                                                    writeRepository,
                                                                    context)
        {
            _roleService = roleService;
        }

        #region IProductGroupService Members

        public async Task<TResponse<IEnumerable<ProductGroupSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if(canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<ProductGroupSimpleModel>(SqlQuery.PRODUCT_GROUP_GET_ALL);
                    if(result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<ProductGroupSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<ProductGroupSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<ProductGroupSimpleModel>>(exception);
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

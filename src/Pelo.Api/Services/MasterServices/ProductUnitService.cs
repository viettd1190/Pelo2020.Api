using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.ProductUnit;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IProductUnitService
    {
        Task<TResponse<IEnumerable<ProductUnitSimpleModel>>> GetAll(int userId);
    }

    public class ProductUnitService : BaseService,
                                      IProductUnitService
    {
        readonly IRoleService _roleService;

        public ProductUnitService(IDapperReadOnlyRepository readOnlyRepository,
                                  IDapperWriteRepository writeRepository,
                                  IHttpContextAccessor context,
                                  IRoleService roleService) : base(readOnlyRepository,
                                                                   writeRepository,
                                                                   context)
        {
            _roleService = roleService;
        }

        #region IProductUnitService Members

        public async Task<TResponse<IEnumerable<ProductUnitSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if(canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<ProductUnitSimpleModel>(SqlQuery.PRODUCT_UNIT_GET_ALL);
                    if(result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<ProductUnitSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<ProductUnitSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<ProductUnitSimpleModel>>(exception);
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

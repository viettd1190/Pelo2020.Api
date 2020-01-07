using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Product;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IProductService
    {
        Task<TResponse<IEnumerable<ProductSimpleModel>>> GetAll(int userId);

        Task<ProductSimpleModel> GetSimpleById(int id);
    }

    public class ProductService : BaseService,
                                  IProductService
    {
        readonly IRoleService _roleService;

        public ProductService(IDapperReadOnlyRepository readOnlyRepository,
                              IDapperWriteRepository writeRepository,
                              IHttpContextAccessor context,
                              IRoleService roleService) : base(readOnlyRepository,
                                                               writeRepository,
                                                               context)
        {
            _roleService = roleService;
        }

        #region IProductService Members

        public async Task<TResponse<IEnumerable<ProductSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if(canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<ProductSimpleModel>(SqlQuery.PRODUCT_GET_ALL);
                    if(result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<ProductSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<ProductSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<ProductSimpleModel>>(exception);
            }
        }

        public async Task<ProductSimpleModel> GetSimpleById(int id)
        {
            try
            {
                var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<ProductSimpleModel>(SqlQuery.PRODUCT_GET_SIMPLE_BY_ID,
                                                                                                   new
                                                                                                   {
                                                                                                           Id = id
                                                                                                   });
                if(result.IsSuccess
                   && result.Data != null)
                {
                    return result.Data;
                }
            }
            catch (Exception exception)
            {
                //
            }

            return null;
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

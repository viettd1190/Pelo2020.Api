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

        Task<TResponse<PageResult<GetProductPagingResponse>>> GetPaging(int userId, GetProductPagingRequest request);

        Task<TResponse<ProductModel>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertProduct request);

        Task<TResponse<bool>> Update(int userId, UpdateProduct request);

        Task<TResponse<bool>> Delete(int userId, int id);
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

        public async Task<TResponse<bool>> Delete(int userId, int id)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PRODUCT_GET_ALL,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                                  UserUpdated = userId,
                                                                                                                  DateUpdated = DateTime.Now
                                                                                                              });
                        if (result.IsSuccess)
                        {
                            return await Ok(true);
                        }
                        return await Fail<bool>(result.Message);
                    }
                    return await Fail<bool>(data.Message);
                }
                return await Fail<bool>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
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

        public async Task<TResponse<ProductModel>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<ProductModel>(SqlQuery.PRODUCT_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<ProductModel>(result.Message);
                }

                return await Fail<ProductModel>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<ProductModel>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetProductPagingResponse>>> GetPaging(int userId, GetProductPagingRequest request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetProductPagingResponse, int>(SqlQuery.PRODUCT_GET_BY_PAGING,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetProductPagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetProductPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetProductPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetProductPagingResponse>>(exception);
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

        public async Task<TResponse<bool>> Insert(int userId, InsertProduct request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PRODUCT_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  request.ImportPrice,
                                                                                                                  request.SellPrice,
                                                                                                                  request.MinCount,
                                                                                                                  request.MaxCount,
                                                                                                                  request.WarrantyMonth,
                                                                                                                  request.ProductStatusId,
                                                                                                                  request.ProductGroupId,
                                                                                                                  request.ProductUnitId,
                                                                                                                  request.ManufacturerId,
                                                                                                                  request.CountryId,
                                                                                                                  request.Description,
                                                                                                                  UserCreated = userId,
                                                                                                                  DateCreated = DateTime.Now,
                                                                                                                  UserUpdated = userId,
                                                                                                                  DateUpdated = DateTime.Now
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(true);
                    }
                    return await Fail<bool>(result.Message);
                }
                return await Fail<bool>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            };
        }

        public async Task<TResponse<bool>> Update(int userId, UpdateProduct request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PRODUCT_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.Name,
                                                                                                                  request.ImportPrice,
                                                                                                                  request.SellPrice,
                                                                                                                  request.MinCount,
                                                                                                                  request.MaxCount,
                                                                                                                  request.WarrantyMonth,
                                                                                                                  request.ProductStatusId,
                                                                                                                  request.ProductGroupId,
                                                                                                                  request.ProductUnitId,
                                                                                                                  request.ManufacturerId,
                                                                                                                  request.CountryId,
                                                                                                                  request.Description,
                                                                                                                  UserUpdated = userId,
                                                                                                                  DateUpdated = DateTime.Now,
                                                                                                              });
                        if (result.IsSuccess)
                        {
                            return await Ok(true);
                        }
                        return await Fail<bool>(result.Message);
                    }
                    return await Fail<bool>(data.Message);
                }
                return await Fail<bool>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
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

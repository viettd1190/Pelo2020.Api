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

        Task<TResponse<PageResult<GetProductUnitPagingResponse>>> GetPaging(int userId, GetProductUnitPagingRequest request);

        Task<TResponse<GetProductUnitReponse>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertProductUnit request);

        Task<TResponse<bool>> Update(int userId, UpdateProductUnit request);

        Task<TResponse<bool>> Delete(int userId, int id);
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PRODUCT_UNIT_DELETE,
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

        #region IProductUnitService Members

        public async Task<TResponse<IEnumerable<ProductUnitSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<ProductUnitSimpleModel>(SqlQuery.PRODUCT_UNIT_GET_ALL);
                    if (result.IsSuccess)
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

        public async Task<TResponse<GetProductUnitReponse>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetProductUnitReponse>(SqlQuery.PRODUCT_UNIT_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<GetProductUnitReponse>(result.Message);
                }

                return await Fail<GetProductUnitReponse>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetProductUnitReponse>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetProductUnitPagingResponse>>> GetPaging(int userId, GetProductUnitPagingRequest request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetProductUnitPagingResponse, int>(string.Format(SqlQuery.PRODUCT_UNIT_PAGING,
                                                                                                                         request.ColumnOrder,
                                                                                                                         request.SortDir.ToUpper()),
                                                                                                              new
                                                                                                              {
                                                                                                                  Name = $"%{request.Name}%",
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetProductUnitPagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetProductUnitPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetProductUnitPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetProductUnitPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertProductUnit request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PRODUCT_UNIT_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateProductUnit request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PRODUCT_UNIT_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.Name,
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
                if (checkPermission.IsSuccess)
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

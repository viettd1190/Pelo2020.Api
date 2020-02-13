using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CustomerSource;
using Pelo.Common.Enums;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.CustomerServices
{
    public interface ICustomerSourceService
    {
        Task<TResponse<IEnumerable<CustomerSourceSimpleModel>>> GetAll(int userId);

        Task<TResponse<bool>> Insert(int userId,
                                     InsertCustomerSource request);

        Task<TResponse<bool>> Update(int userId,
                                     UpdateCustomerSource request);

        Task<TResponse<GetCustomerSourceResponse>> GetById(int userId,
                                                         int id);

        Task<TResponse<bool>> Delete(int userId,
                                     int id);

        Task<TResponse<PageResult<GetCustomerSourcePagingResponse>>> GetPaging(int userId, GetCustomerSourcePagingRequest request);

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

        public async Task<TResponse<bool>> Delete(int userId, int id)
        {
            try
            {
                var canDelete = await CheckInvalid(userId,
                                                id);
                if (canDelete.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_SOURCE_DELETE,
                                                                    new
                                                                    {
                                                                        Id = id,
                                                                        UserUpdated = userId,
                                                                        DateUpdated = DateTime.Now
                                                                    });
                    if (result.IsSuccess)
                    {
                        if (result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(result.Message);
                    }

                    return await Fail<bool>(result.Message);
                }

                return await Fail<bool>(canDelete.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        #region ICustomerSourceService Members

        public async Task<TResponse<IEnumerable<CustomerSourceSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<CustomerSourceSimpleModel>(SqlQuery.CUSTOMER_SOURCE_GET_ALL);
                    if (result.IsSuccess)
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

        public async Task<TResponse<GetCustomerSourceResponse>> GetById(int userId, int id)
        {
            try
            {
                var canGetById = await CanGetAll(userId);
                if (canGetById.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetCustomerSourceResponse>(SqlQuery.CUSTOMER_SOURCE_GET_BY_ID,
                                                                                                            new
                                                                                                            {
                                                                                                                Id = id
                                                                                                            });
                    if (result.IsSuccess)
                    {
                        if (result.Data != null)
                        {
                            return await Ok(result.Data);
                        }

                        return await Fail<GetCustomerSourceResponse>(result.Message);
                    }

                    return await Fail<GetCustomerSourceResponse>(result.Message);
                }

                return await Fail<GetCustomerSourceResponse>(canGetById.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetCustomerSourceResponse>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetCustomerSourcePagingResponse>>> GetPaging(int userId, GetCustomerSourcePagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCustomerSourcePagingResponse, int>(SqlQuery.CUSTOMER_SOURCE_GET_BY_PAGING,
                                                                                                              new
                                                                                                              {
                                                                                                                  Name = $"%{request.Name}%",
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetCustomerSourcePagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetCustomerSourcePagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetCustomerSourcePagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCustomerSourcePagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertCustomerSource request)
        {
            try
            {
                var canInsert = await CanGetAll(userId);
                if (canInsert.IsSuccess)
                {

                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_SOURCE_INSERT,
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
                        if (result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(result.Message);
                    }

                    return await Fail<bool>(result.Message);
                }

                return await Fail<bool>(canInsert.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        public async Task<TResponse<bool>> Update(int userId, UpdateCustomerSource request)
        {
            try
            {
                var canUpdate = await CheckInvalid(userId,
                                                request.Id);
                if (canUpdate.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_SOURCE_UPDATE,
                                                                new
                                                                {
                                                                    request.Id,
                                                                    request.Name,
                                                                    UserUpdated = userId,
                                                                    DateUpdated = DateTime.Now
                                                                });
                    if (result.IsSuccess)
                    {
                        if (result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(result.Message);
                    }
                }

                return await Fail<bool>(canUpdate.Message);
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

        private async Task<TResponse<bool>> CheckInvalid(int userId,
                                                      int id)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if (checkPermission.IsSuccess)
                {
                    var checkIdInvalid = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_SOURCE_GET_BY_ID,
                                                                                                new
                                                                                                {
                                                                                                    Id = id
                                                                                                });
                    if (checkIdInvalid.IsSuccess)
                    {
                        if (checkIdInvalid.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(checkIdInvalid.Message);
                    }

                    return await Fail<bool>(checkIdInvalid.Message);
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

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CustomerGroup;
using Pelo.Common.Enums;
using Pelo.Common.Extensions;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.CustomerServices
{
    public interface ICustomerGroupService
    {
        Task<TResponse<PageResult<GetCustomerGroupPagingResponse>>> GetPaging(int userId,
                                                                              GetCustomerGroupPagingRequest request);

        Task<TResponse<bool>> Insert(int userId,
                                     InsertCustomerGroupRequest request);

        Task<TResponse<bool>> Update(int userId,
                                     UpdateCustomerGroupRequest request);

        Task<TResponse<GetCustomerGroupByIdResponse>> GetById(int userId,
                                                              int id);

        Task<TResponse<bool>> Delete(int userId,
                                     int id);
    }

    public class CustomerGroupService : BaseService,
                                        ICustomerGroupService
    {
        private readonly IRoleService _roleService;

        public CustomerGroupService(IDapperReadOnlyRepository readOnlyRepository,
                                    IDapperWriteRepository writeRepository,
                                    IHttpContextAccessor context,
                                    IRoleService roleService) : base(readOnlyRepository,
                                                                     writeRepository,
                                                                     context)
        {
            _roleService = roleService;
        }

        #region ICustomerGroupService Members

        public async Task<TResponse<PageResult<GetCustomerGroupPagingResponse>>> GetPaging(int userId,
                                                                                           GetCustomerGroupPagingRequest request)
        {
            try
            {
                var canGetPaging = await CanGetPaging(userId);
                if(canGetPaging.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCustomerGroupPagingResponse, int>(string.Format(SqlQuery.CUSTOMER_GROUP_GET_BY_PAGING,
                                                                                                                                  request.ColumnOrder,
                                                                                                                                  request.SortDir.ToUpper()),
                                                                                                                    new
                                                                                                                    {
                                                                                                                            Name = $"%{request.Name}%",
                                                                                                                            Skip = (request.Page - 1) * request.PageSize,
                                                                                                                            Take = request.PageSize
                                                                                                                    });
                    if(result.IsSuccess)
                        return await Ok(new PageResult<GetCustomerGroupPagingResponse>(request.Page,
                                                                                       request.PageSize,
                                                                                       result.Data.Item2,
                                                                                       result.Data.Item1));

                    return await Fail<PageResult<GetCustomerGroupPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetCustomerGroupPagingResponse>>(canGetPaging.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCustomerGroupPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId,
                                                  InsertCustomerGroupRequest request)
        {
            try
            {
                var canInsert = await CanInsert(userId,
                                                request);
                if(canInsert.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_GROUP_INSERT,
                                                                    new
                                                                    {
                                                                            request.Name,
                                                                            UserCreated = userId,
                                                                            UserUpdated = userId
                                                                    });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                              "CUSTOMER_GROUP_INSERT"));
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

        public async Task<TResponse<bool>> Update(int userId,
                                                  UpdateCustomerGroupRequest request)
        {
            try
            {
                var canUpdate = await CanUpdate(userId,
                                                request);
                if(canUpdate.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_GROUP_UPDATE,
                                                                    new
                                                                    {
                                                                            request.Id,
                                                                            request.Name,
                                                                            UserUpdated = userId
                                                                    });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                              "CUSTOMER_GROUP_UPDATE"));
                    }

                    return await Fail<bool>(result.Message);
                }

                return await Fail<bool>(canUpdate.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        public async Task<TResponse<GetCustomerGroupByIdResponse>> GetById(int userId,
                                                                           int id)
        {
            try
            {
                var canGetById = await CanGetById(userId,
                                                  id);
                if(canGetById.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetCustomerGroupByIdResponse>(SqlQuery.CUSTOMER_GROUP_GET_BY_ID,
                                                                                                                 new
                                                                                                                 {
                                                                                                                         Id = id
                                                                                                                 });
                    if(result.IsSuccess)
                    {
                        if(result.Data != null)
                        {
                            return await Ok(result.Data);
                        }

                        return await Fail<GetCustomerGroupByIdResponse>(ErrorEnum.CUSTOMER_GROUP_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<GetCustomerGroupByIdResponse>(result.Message);
                }

                return await Fail<GetCustomerGroupByIdResponse>(canGetById.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetCustomerGroupByIdResponse>(exception);
            }
        }

        public async Task<TResponse<bool>> Delete(int userId,
                                                  int id)
        {
            try
            {
                var canDelete = await CanDelete(userId,
                                                id);
                if(canDelete.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_GROUP_DELETE,
                                                                    new
                                                                    {
                                                                            Id = id,
                                                                            UserUpdated = userId
                                                                    });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                              "CUSTOMER_GROUP_DELETE"));
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

        #endregion

        private async Task<TResponse<bool>> CanGetPaging(int userId)
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

        private async Task<TResponse<bool>> CanInsert(int userId,
                                                      InsertCustomerGroupRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var checkInvalidName = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_GROUP_CHECK_NAME_INVALID,
                                                                                                  new
                                                                                                  {
                                                                                                          request.Name
                                                                                                  });
                    if(checkInvalidName.IsSuccess)
                    {
                        if(checkInvalidName.Data > 0)
                        {
                            return await Fail<bool>(ErrorEnum.CUSTOMER_GROUP_HAS_EXIST.GetStringValue());
                        }

                        return await Ok(true);
                    }

                    return await Fail<bool>(checkInvalidName.Message);
                }

                return await Fail<bool>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        private async Task<TResponse<bool>> CanUpdate(int userId,
                                                      UpdateCustomerGroupRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var checkInvalidId = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_GROUP_CHECK_ID_INVALID,
                                                                                                new
                                                                                                {
                                                                                                        request.Id
                                                                                                });
                    if(checkInvalidId.IsSuccess)
                    {
                        if(checkInvalidId.Data > 0)
                        {
                            var checkInvalidName = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_GROUP_CHECK_NAME_INVALID_2,
                                                                                                          new
                                                                                                          {
                                                                                                                  request.Name,
                                                                                                                  request.Id
                                                                                                          });
                            if(checkInvalidName.IsSuccess)
                            {
                                if(checkInvalidName.Data > 0)
                                {
                                    return await Fail<bool>(ErrorEnum.CUSTOMER_GROUP_HAS_EXIST.GetStringValue());
                                }

                                return await Ok(true);
                            }

                            return await Fail<bool>(checkInvalidName.Message);
                        }

                        return await Fail<bool>(ErrorEnum.CUSTOMER_GROUP_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<bool>(checkInvalidId.Message);
                }

                return await Fail<bool>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        private async Task<TResponse<bool>> CanGetById(int userId,
                                                       int id)
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

        private async Task<TResponse<bool>> CanDelete(int userId,
                                                      int id)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var checkIdInvalid = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_GROUP_CHECK_ID_INVALID,
                                                                                                new
                                                                                                {
                                                                                                        Id = id
                                                                                                });
                    if(checkIdInvalid.IsSuccess)
                    {
                        if(checkIdInvalid.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(ErrorEnum.CUSTOMER_GROUP_HAS_NOT_EXIST.GetStringValue());
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

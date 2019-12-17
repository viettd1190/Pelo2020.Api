using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CustomerVip;
using Pelo.Common.Enums;
using Pelo.Common.Extensions;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.CustomerServices
{
    public interface ICustomerVipService
    {
        Task<TResponse<PageResult<GetCustomerVipPagingResponse>>> GetPaging(int userId,
                                                                            GetCustomerVipPagingRequest request);

        Task<TResponse<bool>> Insert(int userId,
                                     InsertCustomerVipRequest request);

        Task<TResponse<bool>> Update(int userId,
                                     UpdateCustomerVipRequest request);

        Task<TResponse<GetCustomerVipByIdResponse>> GetById(int userId,
                                                            int id);

        Task<TResponse<bool>> Delete(int userId,
                                     int id);
    }

    public class CustomerVipService : BaseService,
                                      ICustomerVipService
    {
        private readonly IRoleService _roleService;

        public CustomerVipService(IDapperReadOnlyRepository readOnlyRepository,
                                  IDapperWriteRepository writeRepository,
                                  IHttpContextAccessor context,
                                  IRoleService roleService) : base(readOnlyRepository,
                                                                   writeRepository,
                                                                   context)
        {
            _roleService = roleService;
        }

        #region ICustomerVipService Members

        public async Task<TResponse<PageResult<GetCustomerVipPagingResponse>>> GetPaging(int userId,
                                                                                         GetCustomerVipPagingRequest request)
        {
            try
            {
                var canGetPaging = await CanGetPaging(userId);
                if(canGetPaging.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCustomerVipPagingResponse, int>(string.Format(SqlQuery.CUSTOMER_VIP_GET_BY_PAGING,
                                                                                                                                request.ColumnOrder,
                                                                                                                                request.SortDir.ToUpper()),
                                                                                                                  new
                                                                                                                  {
                                                                                                                          Skip = (request.Page - 1) * request.PageSize,
                                                                                                                          Take = request.PageSize
                                                                                                                  });
                    if(result.IsSuccess)
                        return await Ok(new PageResult<GetCustomerVipPagingResponse>(request.Page,
                                                                                     request.PageSize,
                                                                                     result.Data.Item2,
                                                                                     result.Data.Item1));

                    return await Fail<PageResult<GetCustomerVipPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetCustomerVipPagingResponse>>(canGetPaging.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCustomerVipPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId,
                                                  InsertCustomerVipRequest request)
        {
            try
            {
                var canInsert = await CanInsert(userId,
                                                request);
                if(canInsert.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_VIP_INSERT,
                                                                    new
                                                                    {
                                                                            request.Name,
                                                                            request.Color,
                                                                            request.Profit,
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
                                                              "CUSTOMER_VIP_INSERT"));
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
                                                  UpdateCustomerVipRequest request)
        {
            try
            {
                var canUpdate = await CanUpdate(userId,
                                                request);
                if(canUpdate.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_VIP_UPDATE,
                                                                    new
                                                                    {
                                                                            request.Id,
                                                                            request.Name,
                                                                            request.Color,
                                                                            request.Profit,
                                                                            UserUpdated = userId
                                                                    });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                              "CUSTOMER_VIP_UPDATE"));
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

        public async Task<TResponse<GetCustomerVipByIdResponse>> GetById(int userId,
                                                                         int id)
        {
            try
            {
                var canGetById = await CanGetById(userId,
                                                  id);
                if(canGetById.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetCustomerVipByIdResponse>(SqlQuery.CUSTOMER_VIP_GET_BY_ID,
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

                        return await Fail<GetCustomerVipByIdResponse>(ErrorEnum.CUSTOMER_VIP_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<GetCustomerVipByIdResponse>(result.Message);
                }

                return await Fail<GetCustomerVipByIdResponse>(canGetById.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetCustomerVipByIdResponse>(exception);
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
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_VIP_DELETE,
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
                                                              "CUSTOMER_VIP_DELETE"));
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
                                                      InsertCustomerVipRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var checkInvalidName = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_VIP_CHJECK_NAME_INVALID,
                                                                                                  new
                                                                                                  {
                                                                                                          request.Name
                                                                                                  });
                    if(checkInvalidName.IsSuccess)
                    {
                        if(checkInvalidName.Data > 0)
                        {
                            return await Fail<bool>(ErrorEnum.CUSTOMER_VIP_HAS_EXIST.GetStringValue());
                        }

                        var checkInvalidProfit = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_VIP_CHECK_PROFIT_INVALID,
                                                                                                        new
                                                                                                        {
                                                                                                                request.Profit
                                                                                                        });

                        if(checkInvalidProfit.IsSuccess)
                        {
                            if(checkInvalidProfit.Data > 0)
                            {
                                return await Fail<bool>(ErrorEnum.CUSTOMER_VIP_PROFIT_HAS_EXIST.GetStringValue());
                            }

                            return await Ok(true);
                        }

                        return await Fail<bool>(checkInvalidProfit.Message);
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
                                                      UpdateCustomerVipRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var checkInvalidId = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_VIP_CHECK_ID_INVALID,
                                                                                                new
                                                                                                {
                                                                                                        request.Id
                                                                                                });
                    if(checkInvalidId.IsSuccess)
                    {
                        var checkInvalidName = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_VIP_CHJECK_NAME_INVALID,
                                                                                                      new
                                                                                                      {
                                                                                                              request.Name
                                                                                                      });
                        if(checkInvalidName.IsSuccess)
                        {
                            if(checkInvalidName.Data > 0)
                            {
                                return await Fail<bool>(ErrorEnum.CUSTOMER_VIP_HAS_EXIST.GetStringValue());
                            }

                            var checkInvalidProfit = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_VIP_CHECK_PROFIT_INVALID,
                                                                                                            new
                                                                                                            {
                                                                                                                    request.Profit
                                                                                                            });

                            if(checkInvalidProfit.IsSuccess)
                            {
                                if(checkInvalidProfit.Data > 0)
                                {
                                    return await Fail<bool>(ErrorEnum.CUSTOMER_VIP_PROFIT_HAS_EXIST.GetStringValue());
                                }

                                return await Ok(true);
                            }

                            return await Fail<bool>(checkInvalidProfit.Message);
                        }

                        return await Fail<bool>(checkInvalidName.Message);
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
                    var checkIdInvalid = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_VIP_CHECK_ID_INVALID,
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

                        return await Fail<bool>(ErrorEnum.CUSTOMER_VIP_HAS_NOT_EXIST.GetStringValue());
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

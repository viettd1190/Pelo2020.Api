using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Customer;
using Pelo.Common.Enums;
using Pelo.Common.Extensions;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.CustomerServices
{
    public interface ICustomerService
    {
        Task<TResponse<PageResult<GetCustomerPagingResponse>>> GetPaging(int userId,
                                                                         GetCustomerPagingRequest request);

        Task<TResponse<bool>> Insert(int userId,
                                     InsertCustomerRequest request);

        Task<TResponse<bool>> Update(int userId,
                                     UpdateCustomerRequest request);

        Task<TResponse<GetCustomerByIdResponse>> GetById(int userId,
                                                         int id);

        Task<TResponse<bool>> Delete(int userId,
                                     int id);

        Task<TResponse<CustomerByPhoneResponse>> GetByPhone(int userId,
                                                            string phone);
    }

    public class CustomerService : BaseService,
                                   ICustomerService
    {
        private readonly ICustomerVipService _customerVipService;

        private readonly IRoleService _roleService;

        public CustomerService(IDapperReadOnlyRepository readOnlyRepository,
                               IDapperWriteRepository writeRepository,
                               IHttpContextAccessor context,
                               IRoleService roleService,
                               ICustomerVipService customerVipService) : base(readOnlyRepository,
                                                                              writeRepository,
                                                                              context)
        {
            _customerVipService = customerVipService;
            _roleService = roleService;
        }

        #region ICustomerService Members

        public async Task<TResponse<PageResult<GetCustomerPagingResponse>>> GetPaging(int userId,
                                                                                      GetCustomerPagingRequest request)
        {
            try
            {
                var canGetPaging = await CanGetPaging(userId);
                if(canGetPaging.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCustomerPagingResponse, int>(string.Format(SqlQuery.CUSTOMER_GET_BY_PAGING,
                                                                                                                             "c." + request.ColumnOrder,
                                                                                                                             request.SortDir.ToUpper()),
                                                                                                               new
                                                                                                               {
                                                                                                                       Code = $"%{request.Code + ""}%",
                                                                                                                       Name = $"%{request.Name + ""}%",
                                                                                                                       ProvinceId = request.ProvinceId ?? 0,
                                                                                                                       DistrictId = request.DistrictId ?? 0,
                                                                                                                       WardId = request.WardId ?? 0,
                                                                                                                       Address = $"%{request.Address + ""}%",
                                                                                                                       Phone = $"%{request.Phone + ""}%",
                                                                                                                       Email = $"%{request.Email + ""}%",
                                                                                                                       request.CustomerGroupId,
                                                                                                                       request.CustomerVipId,
                                                                                                                       Skip = (request.Page - 1) * request.PageSize,
                                                                                                                       Take = request.PageSize
                                                                                                               });
                    if(result.IsSuccess)
                        return await Ok(new PageResult<GetCustomerPagingResponse>(request.Page,
                                                                                  request.PageSize,
                                                                                  result.Data.Item2,
                                                                                  result.Data.Item1));

                    return await Fail<PageResult<GetCustomerPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetCustomerPagingResponse>>(canGetPaging.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCustomerPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId,
                                                  InsertCustomerRequest request)
        {
            try
            {
                var canInsert = await CanInsert(userId,
                                                request);
                if(canInsert.IsSuccess)
                {
                    var code = await AutoCreateCustomerCode();
                    if(string.IsNullOrEmpty(code))
                    {
                        return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                              "Can not create code for customer"));
                    }

                    var customerVipId = await _customerVipService.GetDefault();
                    if(customerVipId.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_INSERT,
                                                                        new
                                                                        {
                                                                                Code = code,
                                                                                request.Name,
                                                                                request.Phone,
                                                                                request.Email,
                                                                                request.WardId,
                                                                                request.Address,
                                                                                request.Description,
                                                                                UserCreated = userId,
                                                                                UserUpdated = userId,
                                                                                request.ProvinceId,
                                                                                request.DistrictId,
                                                                                request.CustomerGroupId,
                                                                                request.Phone2,
                                                                                request.Phone3,
                                                                                CustomerVipId = customerVipId.Data
                                                                        });
                        if(result.IsSuccess)
                        {
                            if(result.Data > 0)
                            {
                                return await Ok(true);
                            }

                            return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                                  "CUSTOMER_INSERT"));
                        }

                        return await Fail<bool>(result.Message);
                    }

                    return await Fail<bool>(customerVipId.Message);
                }

                return await Fail<bool>(canInsert.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        public async Task<TResponse<bool>> Update(int userId,
                                                  UpdateCustomerRequest request)
        {
            try
            {
                var canUpdate = await CanUpdate(userId,
                                                request);
                if(canUpdate.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_UPDATE,
                                                                    new
                                                                    {
                                                                            request.Id,
                                                                            request.Name,
                                                                            request.Phone,
                                                                            request.Phone2,
                                                                            request.Phone3,
                                                                            request.Email,
                                                                            request.ProvinceId,
                                                                            request.DistrictId,
                                                                            request.WardId,
                                                                            request.Address,
                                                                            request.CustomerGroupId,
                                                                            request.Description,
                                                                            UserUpdated = userId
                                                                    });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                              "CUSTOMER_UPDATE"));
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

        public async Task<TResponse<GetCustomerByIdResponse>> GetById(int userId,
                                                                      int id)
        {
            try
            {
                var canGetById = await CanGetById(userId);
                if(canGetById.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetCustomerByIdResponse>(SqlQuery.CUSTOMER_GET_BY_ID,
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

                        return await Fail<GetCustomerByIdResponse>(ErrorEnum.CUSTOMER_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<GetCustomerByIdResponse>(result.Message);
                }

                return await Fail<GetCustomerByIdResponse>(canGetById.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetCustomerByIdResponse>(exception);
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
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CUSTOMER_DELETE,
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
                                                              "CUSTOMER_DELETE"));
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

        public async Task<TResponse<CustomerByPhoneResponse>> GetByPhone(int userId,
                                                                         string phone)
        {
            try
            {
                var canGetByPhone = await CanGetByPhone(userId);
                if(canGetByPhone.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<CustomerByPhoneResponse>(SqlQuery.CUSTOMER_GET_BY_PHONE,
                                                                                                            new
                                                                                                            {
                                                                                                                    Phone = phone
                                                                                                            });
                    if(result.IsSuccess)
                    {
                        if(result.Data != null)
                        {
                            return await Ok(result.Data);
                        }

                        return await Fail<CustomerByPhoneResponse>(ErrorEnum.CUSTOMER_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<CustomerByPhoneResponse>(result.Message);
                }

                return await Fail<CustomerByPhoneResponse>(canGetByPhone.Message);
            }
            catch (Exception exception)
            {
                return await Fail<CustomerByPhoneResponse>(exception);
            }
        }

        #endregion

        private async Task<TResponse<bool>> CanGetByPhone(int userId)
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

        private async Task<TResponse<bool>> CanGetById(int userId)
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

        private async Task<string> AutoCreateCustomerCode()
        {
            try
            {
                var count = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_COUNT_BY_DATE_CREATED,
                                                                                   new
                                                                                   {
                                                                                           DateCreated = DateTime.Today
                                                                                   });
                if(count.IsSuccess)
                {
                    var date = DateTime.Now;
                    return $"KH{date.Year % 100:00}{date.Month:00}{date.Day:00}{(count.Data + 1):000}";
                }
            }
            catch (Exception exception)
            {
                Log(exception);
            }

            return string.Empty;
        }

        private async Task<TResponse<bool>> CanInsert(int userId,
                                                      InsertCustomerRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    if(string.IsNullOrEmpty(request.Name))
                    {
                        return await Fail<bool>(ErrorEnum.CUSTOMER_NAME_IS_NOT_NULL_OR_EMPTY.GetStringValue());
                    }

                    if(string.IsNullOrEmpty(request.Phone)
                       && string.IsNullOrEmpty(request.Phone2)
                       && string.IsNullOrEmpty(request.Phone3))
                    {
                        return await Fail<bool>(ErrorEnum.CUSTOMER_PHONE_IS_NOT_NULL_OR_EMPTY.GetStringValue());
                    }

                    var checkPhoneInvalid = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_CHECK_PHONE_INVALID,
                                                                                                   new
                                                                                                   {
                                                                                                           request.Phone
                                                                                                   });
                    if(checkPhoneInvalid.IsSuccess)
                    {
                        if(checkPhoneInvalid.Data > 0)
                        {
                            return await Fail<bool>(ErrorEnum.CUSTOMER_PHONE_HAS_EXIST.GetStringValue());
                        }

                        return await Ok(true);
                    }

                    return await Fail<bool>(checkPhoneInvalid.Message);
                }

                return await Fail<bool>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        private async Task<TResponse<bool>> CanUpdate(int userId,
                                                      UpdateCustomerRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    if(string.IsNullOrEmpty(request.Name))
                    {
                        return await Fail<bool>(ErrorEnum.CUSTOMER_NAME_IS_NOT_NULL_OR_EMPTY.GetStringValue());
                    }

                    if(string.IsNullOrEmpty(request.Phone)
                       && string.IsNullOrEmpty(request.Phone2)
                       && string.IsNullOrEmpty(request.Phone3))
                    {
                        return await Fail<bool>(ErrorEnum.CUSTOMER_PHONE_IS_NOT_NULL_OR_EMPTY.GetStringValue());
                    }

                    var checkIdInvalid = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_CHECK_ID_INVALID,
                                                                                                new
                                                                                                {
                                                                                                        request.Id
                                                                                                });
                    if(checkIdInvalid.IsSuccess)
                    {
                        if(checkIdInvalid.Data > 0)
                        {
                            var checkPhoneInvalid = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_CHECK_PHONE_INVALID_2,
                                                                                                           new
                                                                                                           {
                                                                                                                   request.Phone,
                                                                                                                   request.Id
                                                                                                           });
                            if(checkPhoneInvalid.IsSuccess)
                            {
                                if(checkPhoneInvalid.Data > 0)
                                {
                                    return await Fail<bool>(ErrorEnum.CUSTOMER_PHONE_HAS_EXIST.GetStringValue());
                                }

                                return await Ok(true);
                            }

                            return await Fail<bool>(checkPhoneInvalid.Message);
                        }

                        return await Fail<bool>(ErrorEnum.CUSTOMER_HAS_NOT_EXIST.GetStringValue());
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

        private async Task<TResponse<bool>> CanDelete(int userId,
                                                      int id)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var checkIdInvalid = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CUSTOMER_CHECK_ID_INVALID,
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.InvoiceStatus;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.InvoiceServices
{
    public interface IInvoiceStatusService
    {
        Task<TResponse<IEnumerable<InvoiceStatusSimpleModel>>> GetAll(int userId);

        Task<TResponse<PageResult<GetInvoiceStatusPagingResponse>>> GetPaging(int userId, GetInvoiceStatusPagingRequest request);

        Task<TResponse<GetInvoiceStatusPagingResponse>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertInvoiceStatus request);

        Task<TResponse<bool>> Update(int userId, UpdateInvoiceStatus request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class InvoiceStatusService : BaseService,
                                        IInvoiceStatusService
    {
        private readonly IRoleService _roleService;

        private readonly IUserService _userService;

        public InvoiceStatusService(IDapperReadOnlyRepository readOnlyRepository,
                                    IDapperWriteRepository writeRepository,
                                    IHttpContextAccessor context,
                                    IUserService userService,
                                    IRoleService roleService) : base(readOnlyRepository,
                                                                     writeRepository,
                                                                     context)
        {
            _roleService = roleService;
            _userService = userService;
        }

        public async Task<TResponse<bool>> Delete(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.INVOICE_STATUS_DELETE,
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

        #region IInvoiceStatusService Members

        public async Task<TResponse<IEnumerable<InvoiceStatusSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<InvoiceStatusSimpleModel>(SqlQuery.INVOICE_STATUS_GET_ALL);
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<InvoiceStatusSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<InvoiceStatusSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<InvoiceStatusSimpleModel>>(exception);
            }
        }

        public async Task<TResponse<GetInvoiceStatusPagingResponse>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetInvoiceStatusPagingResponse>(SqlQuery.INVOICE_STATUS_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<GetInvoiceStatusPagingResponse>(result.Message);
                }

                return await Fail<GetInvoiceStatusPagingResponse>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetInvoiceStatusPagingResponse>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetInvoiceStatusPagingResponse>>> GetPaging(int userId, GetInvoiceStatusPagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetInvoiceStatusPagingResponse, int>(SqlQuery.INVOICE_STATUS_GET_BY_PAGING,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetInvoiceStatusPagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetInvoiceStatusPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetInvoiceStatusPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetInvoiceStatusPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertInvoiceStatus request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.INVOICE_STATUS_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  request.Color,
                                                                                                                  request.IsSendSms,
                                                                                                                  request.SmsContent,
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
            }
        }

        public async Task<TResponse<bool>> Update(int userId, UpdateInvoiceStatus request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.INVOICE_STATUS_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.Name,
                                                                                                                  request.Color,
                                                                                                                  request.IsSendSms,
                                                                                                                  request.SmsContent,
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

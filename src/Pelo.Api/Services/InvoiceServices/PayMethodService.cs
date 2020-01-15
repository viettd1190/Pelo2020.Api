using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.PayMethod;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.InvoiceServices
{
    public interface IPayMethodService
    {
        Task<TResponse<IEnumerable<PayMethodSimpleModel>>> GetAll(int userId);

        Task<TResponse<PageResult<GetPayMethodPagingResponse>>> GetPaging(int userId, GetPayMethodPagingRequest request);

        Task<TResponse<PayMethodModel>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertPayMethod request);

        Task<TResponse<bool>> Update(int userId, UpdatePayMethod request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class PayMethodService : BaseService,
                                    IPayMethodService
    {
        private readonly IRoleService _roleService;

        public PayMethodService(IDapperReadOnlyRepository readOnlyRepository,
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.DEPARTMENT_DELETE,
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

        #region IPayMethodService Members

        public async Task<TResponse<IEnumerable<PayMethodSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if(canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<PayMethodSimpleModel>(SqlQuery.PAY_METHOD_GET_ALL);
                    if(result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<PayMethodSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<PayMethodSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<PayMethodSimpleModel>>(exception);
            }
        }

        public async Task<TResponse<PayMethodModel>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<PayMethodModel>(SqlQuery.PAY_METHOD_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<PayMethodModel>(result.Message);
                }

                return await Fail<PayMethodModel>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PayMethodModel>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetPayMethodPagingResponse>>> GetPaging(int userId, GetPayMethodPagingRequest request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetPayMethodPagingResponse, int>(SqlQuery.PAY_METHOD_GET_BY_PAGING,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetPayMethodPagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetPayMethodPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetPayMethodPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetPayMethodPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertPayMethod request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PAY_METHOD_INSERT,
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

        public async Task<TResponse<bool>> Update(int userId, UpdatePayMethod request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PAY_METHOD_UPDATE,
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

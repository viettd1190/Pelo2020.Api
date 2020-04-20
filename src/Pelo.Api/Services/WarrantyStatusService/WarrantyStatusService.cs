using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Warranty;
using Pelo.Common.Dtos.WarrantyStatus;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.WarrantyServices
{
    public interface IWarrantyStatusService
    {
        Task<TResponse<IEnumerable<WarrantyStatusSimpleModel>>> GetAll(int userId);

        Task<TResponse<PageResult<GetWarrantyStatusPagingResponse>>> GetPaging(int userId, GetWarrantyStatusPagingRequest request);

        Task<TResponse<GetWarrantyStatusResponse>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertWarrantyStatus request);

        Task<TResponse<bool>> Update(int userId, UpdateWarrantyStatus request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }
    public class WarrantyStatusService : BaseService, IWarrantyStatusService
    {
        private readonly IAppConfigService _appConfigService;

        private readonly IRoleService _roleService;

        private readonly IUserService _userService;

        private IProductService _productService;
        public WarrantyStatusService(IDapperReadOnlyRepository readOnlyRepository, IDapperWriteRepository writeRepository, IHttpContextAccessor context, IRoleService roleService,
                              IUserService userService,
                              IProductService productService,
                              IAppConfigService appConfigService) : base(readOnlyRepository, writeRepository, context)
        {
            _roleService = roleService;
            _appConfigService = appConfigService;
            _userService = userService;
            _productService = productService;
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.WARRANTY_STATUS_DELETE,
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

        public async Task<TResponse<IEnumerable<WarrantyStatusSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<WarrantyStatusSimpleModel>(SqlQuery.WARRANTY_STATUS_GET_ALL);
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<WarrantyStatusSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<WarrantyStatusSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<WarrantyStatusSimpleModel>>(exception);
            }
        }

        public async Task<TResponse<GetWarrantyStatusResponse>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetWarrantyStatusResponse>(SqlQuery.WARRANTY_STATUS_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<GetWarrantyStatusResponse>(result.Message);
                }

                return await Fail<GetWarrantyStatusResponse>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetWarrantyStatusResponse>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetWarrantyStatusPagingResponse>>> GetPaging(int userId, GetWarrantyStatusPagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetWarrantyStatusPagingResponse, int>(string.Format(SqlQuery.WARRANTY_STATUS_GET_BY_PAGING, request.ColumnOrder, request.SortDir.ToUpper()),
                                                                                                              new
                                                                                                              {
                                                                                                                  Name =$"%{request.Name}%",
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetWarrantyStatusPagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetWarrantyStatusPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetWarrantyStatusPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetWarrantyStatusPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertWarrantyStatus request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.WARRANTY_STATUS_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  request.Color,
                                                                                                                  request.SortOrder,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateWarrantyStatus request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.WARRANTY_STATUS_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.Name,
                                                                                                                  request.Color,
                                                                                                                  request.SortOrder,
                                                                                                                  request.IsSendSms,
                                                                                                                  request.SmsContent,
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

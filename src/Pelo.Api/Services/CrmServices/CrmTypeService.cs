using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.CrmType;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.CrmServices
{
    public interface ICrmTypeService
    {
        Task<TResponse<IEnumerable<CrmTypeSimpleModel>>> GetAll(int userId);

        Task<TResponse<PageResult<GetCrmTypePagingResponse>>> GetPaging(int v, GetCrmTypePagingRequest request);

        Task<TResponse<GetCrmTypeResponse>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertCrmType request);

        Task<TResponse<bool>> Update(int userId, UpdateCrmType request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class CrmTypeService : BaseService,
                                  ICrmTypeService
    {
        readonly IRoleService _roleService;

        public CrmTypeService(IDapperReadOnlyRepository readOnlyRepository,
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
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.CRM_TYPE_DELETE,
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

        #region ICrmTypeService Members

        public async Task<TResponse<IEnumerable<CrmTypeSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<CrmTypeSimpleModel>(SqlQuery.CRM_TYPE_GET_ALL);
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<CrmTypeSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<CrmTypeSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<CrmTypeSimpleModel>>(exception);
            }
        }

        public async Task<TResponse<GetCrmTypeResponse>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetCrmTypeResponse>(SqlQuery.CRM_TYPE_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<GetCrmTypeResponse>(result.Message);
                }

                return await Fail<GetCrmTypeResponse>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetCrmTypeResponse>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetCrmTypePagingResponse>>> GetPaging(int userId, GetCrmTypePagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCrmTypePagingResponse, int>(SqlQuery.CRM_TYPE_GET_BY_PAGING,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetCrmTypePagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetCrmTypePagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetCrmTypePagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCrmTypePagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertCrmType request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.CRM_TYPE_INSERT,
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
            }
        }

        public async Task<TResponse<bool>> Update(int userId, UpdateCrmType request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.CRM_TYPE_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.Name,
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

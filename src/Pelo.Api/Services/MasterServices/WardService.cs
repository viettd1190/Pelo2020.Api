using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Ward;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IWardService
    {
        Task<TResponse<IEnumerable<WardModel>>> GetAll(int districtId);

        Task<TResponse<PageResult<GetWardPagingResponse>>> GetPaging(int userId, GetWardPagingRequest request);

        Task<TResponse<WardModel>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertWard request);

        Task<TResponse<bool>> Update(int userId, UpdateWard request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class WardService : BaseService,
                                   IWardService
    {
        private readonly IRoleService _roleService;

        public WardService(IDapperReadOnlyRepository readOnlyRepository,
                               IDapperWriteRepository writeRepository,
                               IHttpContextAccessor context, IRoleService roleService) : base(readOnlyRepository,
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.WARD_DELETE,
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

        #region IWardService Members

        public async Task<TResponse<IEnumerable<WardModel>>> GetAll(int districtId)
        {
            try
            {
                var result = await ReadOnlyRepository.Query<WardModel>(SqlQuery.WARD_GET_ALL,
                                                                           new
                                                                           {
                                                                                   DistrictId = districtId
                                                                           });
                if(result.IsSuccess)
                {
                    return await Ok(result.Data);
                }

                return await Fail<IEnumerable<WardModel>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<WardModel>>(exception);
            }
        }

        public async Task<TResponse<WardModel>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<WardModel>(SqlQuery.WARD_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<WardModel>(result.Message);
                }

                return await Fail<WardModel>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<WardModel>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetWardPagingResponse>>> GetPaging(int userId, GetWardPagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetWardPagingResponse, int>(SqlQuery.WARD_PAGING,
                                                                                                              new
                                                                                                              {
                                                                                                                  Name = $"%{request.Name}%",
                                                                                                                  request.ProvinceId,
                                                                                                                  request.DistrictId,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetWardPagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetWardPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetWardPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetWardPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertWard request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.WARD_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Type,
                                                                                                                  request.Name,
                                                                                                                  request.ProvinceId,
                                                                                                                  request.DistrictId,
                                                                                                                  request.SortOrder,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateWard request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.WARD_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.Name,
                                                                                                                  request.Type,
                                                                                                                  request.DistrictId,
                                                                                                                  request.ProvinceId,
                                                                                                                  request.SortOrder,
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

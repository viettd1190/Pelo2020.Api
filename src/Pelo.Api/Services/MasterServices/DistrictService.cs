using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.District;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IDistrictService
    {
        Task<TResponse<IEnumerable<DistrictModel>>> GetAll(int provinceId);

        Task<TResponse<PageResult<GetDistrictPagingResponse>>> GetPaging(int userId, GetDistrictPagingRequest request);

        Task<TResponse<DistrictModel>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertDistrict request);

        Task<TResponse<bool>> Update(int userId, UpdateDistrict request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class DistrictService : BaseService,
                                   IDistrictService
    {
        private readonly IRoleService _roleService;

        public DistrictService(IDapperReadOnlyRepository readOnlyRepository,
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
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PROVINCE_DELETE,
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

        #region IDistrictService Members

        public async Task<TResponse<IEnumerable<DistrictModel>>> GetAll(int provinceId)
        {
            try
            {
                var result = await ReadOnlyRepository.Query<DistrictModel>(SqlQuery.DISTRICT_GET_ALL,
                                                                           new
                                                                           {
                                                                                   ProvinceId = provinceId
                                                                           });
                if(result.IsSuccess)
                {
                    return await Ok(result.Data);
                }

                return await Fail<IEnumerable<DistrictModel>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<DistrictModel>>(exception);
            }
        }

        public async Task<TResponse<DistrictModel>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<DistrictModel>(SqlQuery.DISTRICT_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<DistrictModel>(result.Message);
                }

                return await Fail<DistrictModel>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<DistrictModel>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetDistrictPagingResponse>>> GetPaging(int userId, GetDistrictPagingRequest request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetDistrictPagingResponse, int>(string.Format(SqlQuery.DISTRICT_PAGING,
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
                        return await Ok(new PageResult<GetDistrictPagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetDistrictPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetDistrictPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetDistrictPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertDistrict request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.ROLE_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Type,
                                                                                                                  request.Name,
                                                                                                                  request.ProvinceId,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateDistrict request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.ROLE_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.Name,
                                                                                                                  request.Type,
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
    }
}

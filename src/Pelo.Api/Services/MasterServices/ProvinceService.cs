using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Province;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IProvinceService
    {
        Task<TResponse<IEnumerable<ProvinceModel>>> GetAll();

        Task<TResponse<PageResult<GetProvincePagingResponse>>> GetPaging(int userId, GetProvincePagingRequest request);

        Task<TResponse<ProvinceModel>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertProvince request);

        Task<TResponse<bool>> Update(int userId, UpdateProvince request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class ProvinceService : BaseService,
                                   IProvinceService
    {
        private readonly IRoleService _roleService;

        public ProvinceService(IDapperReadOnlyRepository readOnlyRepository,
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

        #region IProvinceService Members

        public async Task<TResponse<IEnumerable<ProvinceModel>>> GetAll()
        {
            try
            {
                var result = await ReadOnlyRepository.Query<ProvinceModel>(SqlQuery.PROVINCE_GET_ALL);
                if(result.IsSuccess)
                {
                    return await Ok(result.Data);
                }

                return await Fail<IEnumerable<ProvinceModel>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<ProvinceModel>>(exception);
            }
        }

        public async Task<TResponse<ProvinceModel>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<ProvinceModel>(SqlQuery.PROVINCE_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<ProvinceModel>(result.Message);
                }

                return await Fail<ProvinceModel>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<ProvinceModel>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetProvincePagingResponse>>> GetPaging(int userId, GetProvincePagingRequest request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetProvincePagingResponse, int>(SqlQuery.PROVINCE_PAGING,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetProvincePagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetProvincePagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetProvincePagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetProvincePagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertProvince request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PROVINCE_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Type,
                                                                                                                  request.Name,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateProvince request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PROVINCE_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.Name,
                                                                                                                  request.Type,
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

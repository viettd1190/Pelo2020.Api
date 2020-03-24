using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Country;
using Pelo.Common.Dtos.CrmType;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.CountryServices
{
    public interface ICountryService
    {
        Task<TResponse<IEnumerable<GetCountryReponse>>> GetAll(int userId);

        Task<TResponse<PageResult<GetCountryPagingResponse>>> GetPaging(int v, GetCountryPagingRequest request);

        Task<TResponse<GetCountryReponse>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertCountry request);

        Task<TResponse<bool>> Update(int userId, UpdateCountry request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class CountryService : BaseService,
                                  ICountryService
    {
        readonly IRoleService _roleService;

        public CountryService(IDapperReadOnlyRepository readOnlyRepository,
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.COUNTRY_DELETE,
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

        public async Task<TResponse<IEnumerable<GetCountryReponse>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<GetCountryReponse>(SqlQuery.COUNTRY_GET_ALL);
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<GetCountryReponse>>(result.Message);
                }

                return await Fail<IEnumerable<GetCountryReponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<GetCountryReponse>>(exception);
            }
        }

        public async Task<TResponse<GetCountryReponse>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetCountryReponse>(SqlQuery.COUNTRY_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<GetCountryReponse>(result.Message);
                }

                return await Fail<GetCountryReponse>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetCountryReponse>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetCountryPagingResponse>>> GetPaging(int userId, GetCountryPagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCountryPagingResponse, int>(string.Format(SqlQuery.COUNTRY_GET_BY_PAGING,request.ColumnOrder,request.SortDir),
                                                                                                              new
                                                                                                              {
                                                                                                                  Name = $"%{request.Name}%",
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetCountryPagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetCountryPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetCountryPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCountryPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertCountry request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.COUNTRY_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  request.Code,
                                                                                                                  request.Region,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateCountry request)
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
                                                                                                                  request.Code,
                                                                                                                  request.Region,
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

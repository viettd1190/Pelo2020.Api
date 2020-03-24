using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Manufacturer;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.InvoiceServices
{
    public interface IManufacturerService
    {
        Task<TResponse<IEnumerable<ManufacturerSimpleModel>>> GetAll(int userId);

        Task<TResponse<PageResult<GetManufacturerPagingResponse>>> GetPaging(int userId, GetManufacturerPagingRequest request);

        Task<TResponse<GetManufacturerResponse>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertManufacturerRequest request);

        Task<TResponse<bool>> Update(int userId, UpdateManufacturerRequest request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class ManufacturerService : BaseService,
                                    IManufacturerService
    {
        private readonly IRoleService _roleService;

        public ManufacturerService(IDapperReadOnlyRepository readOnlyRepository,
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.MANUFACTURER_DELETE,
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

        #region IManufacturerService Members

        public async Task<TResponse<IEnumerable<ManufacturerSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if(canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<ManufacturerSimpleModel>(SqlQuery.MANUFACTURER_GET_ALL);
                    if(result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<ManufacturerSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<ManufacturerSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<ManufacturerSimpleModel>>(exception);
            }
        }

        public async Task<TResponse<GetManufacturerResponse>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetManufacturerResponse>(SqlQuery.MANUFACTURER_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<GetManufacturerResponse>(result.Message);
                }

                return await Fail<GetManufacturerResponse>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetManufacturerResponse>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetManufacturerPagingResponse>>> GetPaging(int userId, GetManufacturerPagingRequest request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetManufacturerPagingResponse, int>(string.Format(SqlQuery.MANUFACTURER_PAGING,
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
                        return await Ok(new PageResult<GetManufacturerPagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetManufacturerPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetManufacturerPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetManufacturerPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertManufacturerRequest request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.MANUFACTURER_INSERT,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateManufacturerRequest request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.MANUFACTURER_UPDATE,
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.WarrantyDescription;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.WarrantyServices
{
    public interface IWarrantyDescriptionService
    {
        Task<TResponse<IEnumerable<WarrantyDescriptionSimpleModel>>> GetAll(int userId);
        Task<TResponse<PageResult<GetWarrantyDescriptionPagingResponse>>> GetPaging(int v, GetWarrantyDescriptionPagingRequest request);

        Task<TResponse<WarrantyDescriptionSimpleModel>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertWarrantyDescription request);

        Task<TResponse<bool>> Update(int userId, UpdateWarrantyDescription request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class WarrantyDescriptionService : BaseService,
                                    IWarrantyDescriptionService
    {
        readonly IRoleService _roleService;

        public WarrantyDescriptionService(IDapperReadOnlyRepository readOnlyRepository,
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.WARRANTY_DESCRIPTION_GET_ALL,
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

        #region IWarrantyDescriptionService Members

        public async Task<TResponse<IEnumerable<WarrantyDescriptionSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<WarrantyDescriptionSimpleModel>(SqlQuery.WARRANTY_DESCRIPTION_GET_ALL);
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<WarrantyDescriptionSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<WarrantyDescriptionSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<WarrantyDescriptionSimpleModel>>(exception);
            }
        }

        public async Task<TResponse<WarrantyDescriptionSimpleModel>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<WarrantyDescriptionSimpleModel>(SqlQuery.WARRANTY_DESCRIPTION_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<WarrantyDescriptionSimpleModel>(result.Message);
                }

                return await Fail<WarrantyDescriptionSimpleModel>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<WarrantyDescriptionSimpleModel>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetWarrantyDescriptionPagingResponse>>> GetPaging(int userId, GetWarrantyDescriptionPagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetWarrantyDescriptionPagingResponse, int>(string.Format(SqlQuery.WARRANTY_DESCRIPTION_PAGING,
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
                        return await Ok(new PageResult<GetWarrantyDescriptionPagingResponse>(request.Page,
                                                                                  request.PageSize,
                                                                                  result.Data.Item2,
                                                                                  result.Data.Item1));
                    }

                    return await Fail<PageResult<GetWarrantyDescriptionPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetWarrantyDescriptionPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetWarrantyDescriptionPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertWarrantyDescription request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.WARRANTY_DESCRIPTION_INSERT,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateWarrantyDescription request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.WARRANTY_DESCRIPTION_UPDATE,
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

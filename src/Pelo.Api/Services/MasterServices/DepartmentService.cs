using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Department;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IDepartmentService
    {
        Task<TResponse<IEnumerable<DepartmentSimpleModel>>> GetAll(int userId);

        Task<TResponse<PageResult<GetDepartmentPagingResponse>>> GetPaging(int userId, GetDepartmentPagingRequest request);

        Task<TResponse<GetDepartmentReponse>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertDepartment request);

        Task<TResponse<bool>> Update(int userId, UpdateDepartment request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class DepartmentService : BaseService,
                                     IDepartmentService
    {
        private readonly IRoleService _roleService;

        public DepartmentService(IDapperReadOnlyRepository readOnlyRepository,
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.PAY_METHOD_DELETE,
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

        #region IDepartmentService Members

        public async Task<TResponse<IEnumerable<DepartmentSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<DepartmentSimpleModel>(SqlQuery.DEPARTMENT_GET_ALL);
                    if(result.IsSuccess) return await Ok(result.Data);

                    return await Fail<IEnumerable<DepartmentSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<DepartmentSimpleModel>>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<DepartmentSimpleModel>>(exception);
            }
        }

        public async Task<TResponse<GetDepartmentReponse>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetDepartmentReponse>(SqlQuery.DEPARTMENT_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<GetDepartmentReponse>(result.Message);
                }

                return await Fail<GetDepartmentReponse>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetDepartmentReponse>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetDepartmentPagingResponse>>> GetPaging(int userId, GetDepartmentPagingRequest request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetDepartmentPagingResponse, int>(SqlQuery.DEPARTMENT_PAGING,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetDepartmentPagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetDepartmentPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetDepartmentPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetDepartmentPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertDepartment request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.DEPARTMENT_INSERT,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateDepartment request)
        {
            try
            {
                var canGetAll = await _roleService.CheckPermission(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.DEPARTMENT_UPDATE,
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
    }
}

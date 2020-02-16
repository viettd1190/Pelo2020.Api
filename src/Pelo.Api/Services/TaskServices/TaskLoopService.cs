using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.TaskLoop;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.TaskServices
{
    public interface ITaskLoopService
    {
        Task<TResponse<IEnumerable<TaskLoopSimpleModel>>> GetAll(int userId);
        Task<TResponse<PageResult<GetTaskLoopPagingResponse>>> GetPaging(int v, GetTaskLoopPagingRequest request);

        Task<TResponse<TaskLoopSimpleModel>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertTaskLoop request);

        Task<TResponse<bool>> Update(int userId, UpdateTaskLoop request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class TaskLoopService : BaseService,
                                    ITaskLoopService
    {
        readonly IRoleService _roleService;

        public TaskLoopService(IDapperReadOnlyRepository readOnlyRepository,
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.TASK_LOOP_DELETE,
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

        #region ITaskLoopService Members

        public async Task<TResponse<IEnumerable<TaskLoopSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<TaskLoopSimpleModel>(SqlQuery.TASK_LOOP_GET_ALL);
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<TaskLoopSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<TaskLoopSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<TaskLoopSimpleModel>>(exception);
            }
        }

        public async Task<TResponse<TaskLoopSimpleModel>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<TaskLoopSimpleModel>(SqlQuery.TASK_LOOP_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<TaskLoopSimpleModel>(result.Message);
                }

                return await Fail<TaskLoopSimpleModel>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<TaskLoopSimpleModel>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetTaskLoopPagingResponse>>> GetPaging(int userId, GetTaskLoopPagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetTaskLoopPagingResponse, int>(string.Format(SqlQuery.TASK_LOOP_GET_BY_PAGING,request.ColumnOrder,request.SortDir),
                                                                                                              new
                                                                                                              {
                                                                                                                  Name = $"%{request.Name}%",
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetTaskLoopPagingResponse>(request.Page,
                                                                                  request.PageSize,
                                                                                  result.Data.Item2,
                                                                                  result.Data.Item1));
                    }

                    return await Fail<PageResult<GetTaskLoopPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetTaskLoopPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetTaskLoopPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertTaskLoop request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.TASK_LOOP_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  request.DayCount,
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
            }
        }

        public async Task<TResponse<bool>> Update(int userId, UpdateTaskLoop request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.TASK_LOOP_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.Name,
                                                                                                                  request.DayCount,
                                                                                                                  request.SortOrder,
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Task;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.TaskServices
{
    public interface ITaskService
    {
        //Task<TResponse<IEnumerable<TaskSimpleModel>>> GetAll(int userId);
        Task<TResponse<PageResult<GetTaskPagingResponse>>> GetPaging(int v, GetTaskPagingRequest request);

        Task<TResponse<TaskSimpleModel>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertTask request);

        Task<TResponse<bool>> Update(int userId, UpdateTask request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class TaskService : BaseService,
                                    ITaskService
    {
        readonly IRoleService _roleService;

        public TaskService(IDapperReadOnlyRepository readOnlyRepository,
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.TASK_DELETE,
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

        #region ITaskService Members

        //public async Task<TResponse<IEnumerable<TaskSimpleModel>>> GetAll(int userId)
        //{
        //    try
        //    {
        //        var canGetAll = await CanGetAll(userId);
        //        if (canGetAll.IsSuccess)
        //        {
        //            var result = await ReadOnlyRepository.QueryAsync<TaskSimpleModel>(SqlQuery.TASK_GET_ALL);
        //            if (result.IsSuccess)
        //            {
        //                return await Ok(result.Data);
        //            }

        //            return await Fail<IEnumerable<TaskSimpleModel>>(result.Message);
        //        }

        //        return await Fail<IEnumerable<TaskSimpleModel>>(canGetAll.Message);
        //    }
        //    catch (Exception exception)
        //    {
        //        return await Fail<IEnumerable<TaskSimpleModel>>(exception);
        //    }
        //}

        public async Task<TResponse<TaskSimpleModel>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<TaskSimpleModel>(SqlQuery.TASK_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<TaskSimpleModel>(result.Message);
                }

                return await Fail<TaskSimpleModel>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<TaskSimpleModel>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetTaskPagingResponse>>> GetPaging(int userId, GetTaskPagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetTaskPagingResponse, int>(SqlQuery.TASK_GET_BY_PAGING,
                                                                                                              new
                                                                                                              {
                                                                                                                  Name = $"%{request.Name}%",
                                                                                                                  Phone = $"%{request.Phone}%",
                                                                                                                  Code = $"%{request.Code}%",
                                                                                                                  request.TaskStatusId,
                                                                                                                  request.TaskLoopId,
                                                                                                                  request.TaskPriorityId,
                                                                                                                  request.TaskTypeId,
                                                                                                                  FromDate = $"{request.FromDate}",
                                                                                                                  ToDate = $"{request.ToDate}",
                                                                                                                  request.UserCreatedId,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetTaskPagingResponse>(request.Page,
                                                                                  request.PageSize,
                                                                                  result.Data.Item2,
                                                                                  result.Data.Item1));
                    }

                    return await Fail<PageResult<GetTaskPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetTaskPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetTaskPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertTask request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    string code = $"CV{DateTime.Today.ToString("yyMMdd")}001";
                    var rs = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.TASK_CURRENT_COUNT, new { CurrentDate = $"{DateTime.Today.ToString("yyyy-MM-dd")}" });
                    if (rs.IsSuccess)
                    {
                        code = $"CV{DateTime.Today.ToString("yyMMdd")}{(rs.Data + 1).ToString("D3")}";
                    }
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.TASK_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  Code = code,
                                                                                                                  request.CustomerId,
                                                                                                                  request.Content,
                                                                                                                  request.Description,
                                                                                                                  request.TaskStatusId,
                                                                                                                  request.TaskPriorityId,
                                                                                                                  request.TaskLoopId,
                                                                                                                  request.TaskTypeId,
                                                                                                                  request.FromDateTime,
                                                                                                                  request.ToDateTime,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateTask request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.TASK_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  request.Content,
                                                                                                                  request.Description,
                                                                                                                  request.TaskStatusId,
                                                                                                                  request.TaskPriorityId,
                                                                                                                  request.TaskLoopId,
                                                                                                                  request.TaskTypeId,
                                                                                                                  request.FromDateTime,
                                                                                                                  request.ToDateTime,
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

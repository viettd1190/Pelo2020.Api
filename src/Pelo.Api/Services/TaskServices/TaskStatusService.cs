using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.TaskStatus;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.TaskServices
{
    public interface ITaskStatusService
    {
        Task<TResponse<IEnumerable<TaskStatusSimpleModel>>> GetAll(int userId);
        Task<TResponse<PageResult<GetTaskStatusPagingResponse>>> GetPaging(int v, GetTaskStatusPagingRequest request);

        Task<TResponse<TaskStatusSimpleModel>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertTaskStatus request);

        Task<TResponse<bool>> Update(int userId, UpdateTaskStatus request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class TaskStatusService : BaseService,
                                    ITaskStatusService
    {
        readonly IRoleService _roleService;

        public TaskStatusService(IDapperReadOnlyRepository readOnlyRepository,
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.TASK_STATUS_DELETE,
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

        #region ITaskStatusService Members

        public async Task<TResponse<IEnumerable<TaskStatusSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<TaskStatusSimpleModel>(SqlQuery.TASK_STATUS_GET_ALL);
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<TaskStatusSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<TaskStatusSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<TaskStatusSimpleModel>>(exception);
            }
        }

        public async Task<TResponse<TaskStatusSimpleModel>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<TaskStatusSimpleModel>(SqlQuery.TASK_STATUS_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<TaskStatusSimpleModel>(result.Message);
                }

                return await Fail<TaskStatusSimpleModel>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<TaskStatusSimpleModel>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetTaskStatusPagingResponse>>> GetPaging(int userId, GetTaskStatusPagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetTaskStatusPagingResponse, int>(string.Format(SqlQuery.TASK_STATUS_GET_BY_PAGING,request.ColumnOrder,request.SortDir),
                                                                                                              new
                                                                                                              {
                                                                                                                  Name = $"%{request.Name}%",
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetTaskStatusPagingResponse>(request.Page,
                                                                                  request.PageSize,
                                                                                  result.Data.Item2,
                                                                                  result.Data.Item1));
                    }

                    return await Fail<PageResult<GetTaskStatusPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetTaskStatusPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetTaskStatusPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertTaskStatus request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.TASK_STATUS_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  request.Color,
                                                                                                                  request.SortOrder,
                                                                                                                  request.IsSendSms,
                                                                                                                  request.SmsContent,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateTaskStatus request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.TASK_STATUS_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.Name,
                                                                                                                  request.Color,
                                                                                                                  request.SortOrder,
                                                                                                                  request.IsSendSms,
                                                                                                                  request.SmsContent,
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

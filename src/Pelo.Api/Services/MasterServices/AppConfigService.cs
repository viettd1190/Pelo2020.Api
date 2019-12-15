using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.AppConfig;
using Pelo.Common.Enums;
using Pelo.Common.Extensions;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IAppConfigService
    {
        Task<TResponse<PageResult<GetAppConfigPagingResponse>>> GetPaging(int userId,
                                                                          GetAppConfigPagingRequest request);

        Task<TResponse<bool>> Insert(int userId,
                                     InsertAppConfigRequest request);

        Task<TResponse<bool>> Update(int userId,
                                     UpdateAppConfigRequest request);

        Task<TResponse<GetAppConfigByIdResponse>> GetById(int userId,
                                                          int id);

        Task<TResponse<bool>> Delete(int userId,
                                     int id);

        Task<TResponse<string>> GetByName(string name);
    }

    public class AppConfigService : BaseService,
                                    IAppConfigService
    {
        private readonly IRoleService _roleService;

        public AppConfigService(IDapperReadOnlyRepository readOnlyRepository,
                                IDapperWriteRepository writeRepository,
                                IHttpContextAccessor context,
                                IRoleService roleService) : base(readOnlyRepository,
                                                                 writeRepository,
                                                                 context)
        {
            _roleService = roleService;
        }

        #region IAppConfigService Members

        public async Task<TResponse<PageResult<GetAppConfigPagingResponse>>> GetPaging(int userId,
                                                                                       GetAppConfigPagingRequest request)
        {
            try
            {
                var canGetPaging = await CanGetPaging(userId);
                if(canGetPaging.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetAppConfigPagingResponse, int>(string.Format(SqlQuery.APP_CONFIG_GET_BY_PAGING,
                                                                                                                              request.ColumnOrder,
                                                                                                                              request.SortDir.ToUpper()),
                                                                                                                new
                                                                                                                {
                                                                                                                        Name = $"%{request.Name}%",
                                                                                                                        Description = $"%{request.Description}%",
                                                                                                                        Skip = (request.Page - 1) * request.PageSize,
                                                                                                                        Take = request.PageSize
                                                                                                                });
                    if(result.IsSuccess)
                        return await Ok(new PageResult<GetAppConfigPagingResponse>(request.Page,
                                                                                   request.PageSize,
                                                                                   result.Data.Item2,
                                                                                   result.Data.Item1));

                    return await Fail<PageResult<GetAppConfigPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetAppConfigPagingResponse>>(canGetPaging.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetAppConfigPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId,
                                                  InsertAppConfigRequest request)
        {
            try
            {
                var canInsert = await CanInsert(userId,
                                                request);
                if(canInsert.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.APP_CONFIG_INSERT,
                                                                    new
                                                                    {
                                                                            request.Name,
                                                                            request.Value,
                                                                            request.Description,
                                                                            UserCreated = userId,
                                                                            UserUpdated = userId
                                                                    });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                              "APP_CONFIG_INSERT"));
                    }

                    return await Fail<bool>(result.Message);
                }

                return await Fail<bool>(canInsert.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        public async Task<TResponse<bool>> Update(int userId,
                                                  UpdateAppConfigRequest request)
        {
            try
            {
                var canUpdate = await CanUpdate(userId,
                                                request);
                if(canUpdate.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.APP_CONFIG_UPDATE,
                                                                    new
                                                                    {
                                                                            request.Id,
                                                                            request.Value,
                                                                            request.Description,
                                                                            UserUpdated = userId
                                                                    });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                              "APP_CONFIG_UPDATE"));
                    }

                    return await Fail<bool>(result.Message);
                }

                return await Fail<bool>(canUpdate.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        public async Task<TResponse<GetAppConfigByIdResponse>> GetById(int userId,
                                                                       int id)
        {
            try
            {
                var canGetById = await CanGetById(userId,
                                                  id);
                if(canGetById.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetAppConfigByIdResponse>(SqlQuery.APP_CONFIG_GET_BY_ID,
                                                                                                             new
                                                                                                             {
                                                                                                                     Id = id
                                                                                                             });
                    if(result.IsSuccess)
                    {
                        if(result.Data != null)
                        {
                            return await Ok(result.Data);
                        }

                        return await Fail<GetAppConfigByIdResponse>(ErrorEnum.APP_CONFIG_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<GetAppConfigByIdResponse>(result.Message);
                }

                return await Fail<GetAppConfigByIdResponse>(canGetById.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetAppConfigByIdResponse>(exception);
            }
        }

        public async Task<TResponse<bool>> Delete(int userId,
                                                  int id)
        {
            try
            {
                var canDelete = await CanDelete(userId,
                                                id);
                if(canDelete.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.APP_CONFIG_DELETE,
                                                                    new
                                                                    {
                                                                            Id = id,
                                                                            UserUpdated = userId
                                                                    });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                              "APP_CONFIG_DELETE"));
                    }

                    return await Fail<bool>(result.Message);
                }

                return await Fail<bool>(canDelete.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        public async Task<TResponse<string>> GetByName(string name)
        {
            try
            {
                var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<string>(SqlQuery.APP_CONFIG_GET_VALUE_BY_NAME,
                                                                                       new
                                                                                       {
                                                                                               Name = name
                                                                                       });
                if(result.IsSuccess)
                {
                    return await Ok(result.Data);
                }

                return await Fail<string>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<string>(exception);
            }
        }

        #endregion

        private async Task<TResponse<bool>> CanGetPaging(int userId)
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

        private async Task<TResponse<bool>> CanInsert(int userId,
                                                      InsertAppConfigRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var checkInvalidName = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.APP_CONFIG_CHECK_NAME_INVALID,
                                                                                                  new
                                                                                                  {
                                                                                                          request.Name
                                                                                                  });
                    if(checkInvalidName.IsSuccess)
                    {
                        if(checkInvalidName.Data > 0)
                        {
                            return await Fail<bool>(ErrorEnum.APP_CONFIG_HAS_EXIST.GetStringValue());
                        }

                        return await Ok(true);
                    }

                    return await Fail<bool>(checkInvalidName.Message);
                }

                return await Fail<bool>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        private async Task<TResponse<bool>> CanUpdate(int userId,
                                                      UpdateAppConfigRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var checkInvalidId = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.APP_CONFIG_CHECK_ID_INVALID,
                                                                                                new
                                                                                                {
                                                                                                        request.Id
                                                                                                });
                    if(checkInvalidId.IsSuccess)
                    {
                        if(checkInvalidId.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(ErrorEnum.APP_CONFIG_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<bool>(checkInvalidId.Message);
                }

                return await Fail<bool>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        private async Task<TResponse<bool>> CanGetById(int userId,
                                                       int id)
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

        private async Task<TResponse<bool>> CanDelete(int userId,
                                                      int id)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var checkIdInvalid = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.APP_CONFIG_CHECK_ID_INVALID,
                                                                                                new
                                                                                                {
                                                                                                        Id = id
                                                                                                });
                    if(checkIdInvalid.IsSuccess)
                    {
                        if(checkIdInvalid.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(ErrorEnum.APP_CONFIG_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<bool>(checkIdInvalid.Message);
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

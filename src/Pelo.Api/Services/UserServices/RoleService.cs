using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Common.Dtos.Role;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.UserServices
{
    public interface IRoleService
    {
        Task<TResponse<bool>> CheckPermission(int userId);

        Task<TResponse<string>> GetNameByUserId(int userId);

        Task<TResponse<IEnumerable<RoleSimpleModel>>> GetAll(int userId);
    }

    public class RoleService : BaseService,
                               IRoleService
    {
        public RoleService(IDapperReadOnlyRepository readOnlyRepository,
                           IDapperWriteRepository writeRepository,
                           IHttpContextAccessor contextAccessor) : base(readOnlyRepository,
                                                                        writeRepository,
                                                                        contextAccessor)
        {
        }

        #region IRoleService Members

        public async Task<TResponse<bool>> CheckPermission(int userId)
        {
            //try
            //{
            //    var roleId = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.ROLE_ID_GET_BY_USER_ID, new
            //    {
            //        Id = userId
            //    });
            //    if (roleId.IsSuccess)
            //    {
            //        var rolePermisisonId = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(
            //            SqlQuery.ROLE_CHECK_PERMISSION, new
            //            {
            //                RoleId = roleId.Data,
            //                Controller = Context?.HttpContext?.Request?.Headers["Controller"] ?? string.Empty,
            //                Action = Context?.HttpContext?.Request?.Headers["Action"] ?? string.Empty
            //            });
            //        if (rolePermisisonId.IsSuccess)
            //        {
            //            if (rolePermisisonId.Data > 0) return await Ok(true);

            //            return await Fail<bool>(ErrorEnum.USER_DO_HAVE_NOT_PERMISSON_WITH_ACTION.GetStringValue());
            //        }

            //        return await Fail<bool>(rolePermisisonId.Message);
            //    }

            //    return await Fail<bool>(roleId.Message);
            //}
            //catch (Exception exception)
            //{
            //    return await Fail<bool>(exception);
            //}

            return await Ok(true);
        }

        public async Task<TResponse<string>> GetNameByUserId(int userId)
        {
            try
            {
                var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<string>(SqlQuery.ROLE_NAME_GET_BY_USER_ID,
                                                                                       new
                                                                                       {
                                                                                               UserId = userId
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

        public async Task<TResponse<IEnumerable<RoleSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var checkPermission = await CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<RoleSimpleModel>(SqlQuery.ROLE_GET_ALL);
                    if(result.IsSuccess) return await Ok(result.Data);

                    return await Fail<IEnumerable<RoleSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<RoleSimpleModel>>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<RoleSimpleModel>>(exception);
            }
        }

        #endregion
    }
}

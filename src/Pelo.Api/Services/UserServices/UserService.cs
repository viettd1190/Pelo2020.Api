using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Common.Dtos.User;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.UserServices
{
    public interface IUserService
    {
        Task<TResponse<PageResult<GetUserPagingResponse>>> GetPaging(int userId,
            GetUserPagingRequest request);
    }

    public class UserService : BaseService, IUserService
    {
        private readonly IRoleService _roleService;

        public UserService(IDapperReadOnlyRepository readOnlyRepository, IDapperWriteRepository writeRepository,
            IRoleService roleService, IHttpContextAccessor contextAccessor) : base(
            readOnlyRepository, writeRepository, contextAccessor)
        {
            _roleService = roleService;
        }

        public async Task<TResponse<PageResult<GetUserPagingResponse>>> GetPaging(int userId, GetUserPagingRequest request)
        {
            try
            {
                var canGetPaging = await CanGetPaging(userId);
                if (canGetPaging.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetUserPagingResponse, int>(string.Format(
                            SqlQuery.USER_GET_BY_PAGING,
                            request.ColumnOrder,
                            request.SortDir.ToUpper()),
                        new
                        {
                            Username = $"%{request.Username}%",
                            DisplayName = $"%{request.DisplayName}%",
                            FullName = $"%{request.FullName}%",
                            PhoneNumber = $"%{request.PhoneNumber}%",
                            request.BranchId,
                            request.RoleId,
                            Skip = (request.Page - 1) * request.PageSize,
                            Take = request.PageSize
                        });
                    if (result.IsSuccess)
                        return await Ok(new PageResult<GetUserPagingResponse>(request.Page,
                            request.PageSize,
                            result.Data.Item2,
                            result.Data.Item1));

                    return await Fail<PageResult<GetUserPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetUserPagingResponse>>(canGetPaging.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetUserPagingResponse>>(exception);
            }
        }

        private async Task<TResponse<bool>> CanGetPaging(int userId)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if (checkPermission.IsSuccess) return await Ok(true);

                return await Fail<bool>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }
    }
}
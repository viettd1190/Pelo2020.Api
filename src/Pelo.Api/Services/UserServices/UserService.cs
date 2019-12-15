using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Common.Dtos.User;
using Pelo.Common.Enums;
using Pelo.Common.Extensions;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.UserServices
{
    public interface IUserService
    {
        Task<TResponse<PageResult<GetUserPagingResponse>>> GetPaging(int userId,
                                                                     GetUserPagingRequest request);

        Task<TResponse<bool>> Insert(int userId,
                                     InsertUserRequest request);

        Task<TResponse<bool>> Update(int userId,
                                     UpdateUserRequest request);

        Task<TResponse<GetUserByIdResponse>> GetById(int userId,
                                                     int id);

        Task<TResponse<bool>> Delete(int userId,
                                     int id);
    }

    public class UserService : BaseService,
                               IUserService
    {
        private readonly IRoleService _roleService;

        public UserService(IDapperReadOnlyRepository readOnlyRepository,
                           IDapperWriteRepository writeRepository,
                           IRoleService roleService,
                           IHttpContextAccessor contextAccessor) : base(readOnlyRepository,
                                                                        writeRepository,
                                                                        contextAccessor)
        {
            _roleService = roleService;
        }

        #region IUserService Members

        public async Task<TResponse<PageResult<GetUserPagingResponse>>> GetPaging(int userId,
                                                                                  GetUserPagingRequest request)
        {
            try
            {
                var canGetPaging = await CanGetPaging(userId);
                if(canGetPaging.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetUserPagingResponse, int>(string.Format(SqlQuery.USER_GET_BY_PAGING,
                                                                                                                         request.ColumnOrder,
                                                                                                                         request.SortDir.ToUpper()),
                                                                                                           new
                                                                                                           {
                                                                                                                   Code = $"%{request.Code}%",
                                                                                                                   FullName = $"%{request.FullName}%",
                                                                                                                   PhoneNumber = $"%{request.PhoneNumber}%",
                                                                                                                   request.BranchId,
                                                                                                                   request.DepartmentId,
                                                                                                                   request.RoleId,
                                                                                                                   request.Status,
                                                                                                                   Skip = (request.Page - 1) * request.PageSize,
                                                                                                                   Take = request.PageSize
                                                                                                           });
                    if(result.IsSuccess)
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

        /// <summary>
        ///     Thêm mới 1 user. Các bước thực hiện
        ///     1. Kiểm tra xem đủ điều kiện thêm mới không. Nếu đủ thì làm các bước tiếp theo
        ///     2. Thêm các thông tin cơ bản vào db, ngoại trừ Code. Nếu thêm thành công vào db thì thực hiện các bước tiếp theo
        ///     3. Update code cho user đó, cáu trúc: NV{id:00000}. VD: NV00099
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponse<bool>> Insert(int userId,
                                                  InsertUserRequest request)
        {
            try
            {
                var canInsert = await CanInsert(userId,
                                                request);
                if(canInsert.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.USER_INSERT,
                                                                               new
                                                                               {
                                                                                       request.Username,
                                                                                       Password = Sha512(request.Password),
                                                                                       request.DisplayName,
                                                                                       request.FullName,
                                                                                       request.PhoneNumber,
                                                                                       request.Email,
                                                                                       request.BranchId,
                                                                                       request.RoleId,
                                                                                       request.DepartmentId,
                                                                                       request.Description,
                                                                                       UserCreated = userId,
                                                                                       UserUpdated = userId
                                                                               });
                    if(result.IsSuccess)
                    {
                        if(result.Data == 0)
                        {
                            return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                                  "USER_INSERT"));
                        }

                        int id = result.Data;

                        await WriteRepository.ExecuteAsync(SqlQuery.USER_UPDATE_CODE,
                                                           new
                                                           {
                                                                   Id = id,
                                                                   Code = $"NV{id:00000}"
                                                           });

                        return await Ok(true);
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

        /// <summary>
        ///     Cập nhật thông tin nhân viên
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponse<bool>> Update(int userId,
                                                  UpdateUserRequest request)
        {
            try
            {
                var canUpdate = await CanUpdate(userId,
                                                request);
                if(canUpdate.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.USER_UPDATE,
                                                                    new
                                                                    {
                                                                            request.DisplayName,
                                                                            request.FullName,
                                                                            request.PhoneNumber,
                                                                            request.Email,
                                                                            request.BranchId,
                                                                            request.DepartmentId,
                                                                            request.RoleId,
                                                                            request.IsActive,
                                                                            request.Description,
                                                                            request.Id,
                                                                            UserUpdated = userId
                                                                    });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                              "USER_UPDATE"));
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

        /// <summary>
        ///     Lấy thông tin nhân viên
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TResponse<GetUserByIdResponse>> GetById(int userId,
                                                                  int id)
        {
            try
            {
                var canGetById = await CanGetById(userId,
                                                  id);
                if(canGetById.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetUserByIdResponse>(SqlQuery.USER_GET_BY_ID,
                                                                                                        new
                                                                                                        {
                                                                                                                Id = id
                                                                                                        });
                    if(result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<GetUserByIdResponse>(ErrorEnum.USER_HAS_NOT_EXIST.GetStringValue());
                }

                return await Fail<GetUserByIdResponse>(canGetById.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetUserByIdResponse>(exception);
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
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.USER_DELETE,
                                                                    new
                                                                    {
                                                                            UserUpdated = userId,
                                                                            Id = id
                                                                    });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            return await Ok(true);
                        }

                        return await Fail<bool>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                              "USER_DELETE"));
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

        #endregion

        /// <summary>
        ///     Kiểm tra user có quyền lấy danh sách người dùng hay không
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<TResponse<bool>> CanGetPaging(int userId)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess) return await Ok(true);

                return await Fail<bool>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        /// <summary>
        ///     Kiểm tra khi thêm người dùng phải thỏa mãn tất cả các điều kiện sau:
        ///     1. User phải có quyền thêm người dùng
        ///     2. Không được để trống tên đăng nhập, mật khẩu
        ///     3. Tên đăng nhập không được trùng
        ///     4. Nếu số điện thoại ko để trống, kiểm tra số điện thoại không được trùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<TResponse<bool>> CanInsert(int userId,
                                                      InsertUserRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    if(string.IsNullOrEmpty(request.Username))
                    {
                        return await Fail<bool>(ErrorEnum.USERNAME_IS_NOT_EMPTY.GetStringValue());
                    }

                    if(string.IsNullOrEmpty(request.Password))
                    {
                        return await Fail<bool>(ErrorEnum.PASSWORD_IS_NOT_EMPTY.GetStringValue());
                    }

                    var checkUsername = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.USER_CHECK_USERNAME_INVALID,
                                                                                               new
                                                                                               {
                                                                                                       request.Username
                                                                                               });
                    if(checkUsername.IsSuccess)
                    {
                        if(checkUsername.Data > 0)
                        {
                            return await Fail<bool>(ErrorEnum.USERNAME_HAS_EXIST.GetStringValue());
                        }

                        if(!string.IsNullOrEmpty(request.PhoneNumber))
                        {
                            var checkPhone = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.USER_CHECK_PHONE_INVALID,
                                                                                                    new
                                                                                                    {
                                                                                                            request.PhoneNumber
                                                                                                    });
                            if(checkPhone.IsSuccess)
                            {
                                if(checkPhone.Data > 0)
                                {
                                    return await Fail<bool>(ErrorEnum.PHONE_NUMBER_HAS_EXIST.GetStringValue());
                                }

                                return await Ok(true);
                            }

                            return await Fail<bool>(checkPhone.Message);
                        }

                        return await Ok(true);
                    }

                    return await Fail<bool>(checkUsername.Message);
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

        /// <summary>
        ///     Kiểm tra khi cập nhật thông tin nhân viên phải thỏa mãn các điều kiện sau
        ///     1. User phải có quyền cập nhật thông tin người dùng
        ///     2. Id phải tồn tại
        ///     3. Nếu số điện thoại có điền, thì phải ko trùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<TResponse<bool>> CanUpdate(int userId,
                                                      UpdateUserRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var checkInvalidId = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.USER_CHECK_INVALID_ID,
                                                                                                new
                                                                                                {
                                                                                                        request.Id
                                                                                                });
                    if(checkInvalidId.IsSuccess)
                    {
                        if(checkInvalidId.Data > 0)
                        {
                            if(string.IsNullOrEmpty(request.PhoneNumber))
                            {
                                return await Ok(true);
                            }

                            var checkInvalidPhone = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.USER_CHECK_PHONE_INVALID_2,
                                                                                                           new
                                                                                                           {
                                                                                                                   request.Id,
                                                                                                                   request.PhoneNumber
                                                                                                           });
                            if(checkInvalidPhone.IsSuccess)
                            {
                                if(checkInvalidPhone.Data == 0)
                                {
                                    return await Ok(true);
                                }

                                return await Fail<bool>(ErrorEnum.PHONE_NUMBER_HAS_EXIST.GetStringValue());
                            }

                            return await Fail<bool>(checkInvalidPhone.Message);
                        }

                        return await Fail<bool>(ErrorEnum.USER_HAS_NOT_EXIST.GetStringValue());
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

        /// <summary>
        ///     Kiểm tra khi xóa người dùng phải thỏa mãn tất cả các điều kiện sau:
        ///     1. User phải có quyền xóa người dùng
        ///     2. Người dùng cần xóa phải tồn tại
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<TResponse<bool>> CanDelete(int userId,
                                                      int id)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.USER_CHECK_INVALID_ID,
                                                                                        new
                                                                                        {
                                                                                                Id = id
                                                                                        });
                    if(result.IsSuccess)
                    {
                        if(result.Data == 0)
                        {
                            return await Fail<bool>(ErrorEnum.USER_HAS_NOT_EXIST.GetStringValue());
                        }

                        return await Ok(true);
                    }

                    return await Fail<bool>(result.Message);
                }

                return await Fail<bool>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        private string Sha512(string input)
        {
            input = $"123{input}xyz";

            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hash = SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }
    }
}

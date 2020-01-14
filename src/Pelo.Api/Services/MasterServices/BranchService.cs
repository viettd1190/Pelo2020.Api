using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Branch;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IBranchService
    {
        Task<TResponse<IEnumerable<BranchSimpleModel>>> GetAll(int userId);

        Task<TResponse<PageResult<GetBranchPagingResponse>>> GetPaging(int userId, GetBranchPagingRequest request);

        Task<TResponse<BranchModel>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertBranch request);

        Task<TResponse<bool>> Update(int userId, UpdateBranch request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class BranchService : BaseService,
                                 IBranchService
    {
        private readonly IRoleService _roleService;

        public BranchService(IDapperReadOnlyRepository readOnlyRepository,
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.BRANCH_DELETE,
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

        #region IBranchService Members

        public async Task<TResponse<IEnumerable<BranchSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if (checkPermission.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<BranchSimpleModel>(SqlQuery.BRANCH_GET_ALL);
                    if (result.IsSuccess) return await Ok(result.Data);

                    return await Fail<IEnumerable<BranchSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<BranchSimpleModel>>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<BranchSimpleModel>>(exception);
            }
        }

        public async Task<TResponse<BranchModel>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<BranchModel>(SqlQuery.BRANCH_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<BranchModel>(result.Message);
                }

                return await Fail<BranchModel>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<BranchModel>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetBranchPagingResponse>>> GetPaging(int userId, GetBranchPagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetBranchPagingResponse, int>(SqlQuery.BRANCH_PAGING,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetBranchPagingResponse>(request.Page, request.PageSize, result.Data.Item2, result.Data.Item1));
                    }

                    return await Fail<PageResult<GetBranchPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetBranchPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetBranchPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertBranch request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.BRANCH_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  request.Hotline,
                                                                                                                  request.ProvinceId,
                                                                                                                  request.DistrictId,
                                                                                                                  request.WardId,
                                                                                                                  request.Address,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateBranch request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.BRANCH_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.ProvinceId,
                                                                                                                  request.DistrictId,
                                                                                                                  request.Hotline,
                                                                                                                  request.WardId,
                                                                                                                  request.Address,
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

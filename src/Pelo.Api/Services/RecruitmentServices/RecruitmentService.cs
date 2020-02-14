using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Recruitment;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.TaskServices
{
    public interface IRecruitmentService
    {
        Task<TResponse<PageResult<GetRecruitmentPagingResponse>>> GetPaging(int v, GetRecruitmentPagingRequest request);

        Task<TResponse<RecruitmentSimpleModel>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertRecruitment request);

        Task<TResponse<bool>> Update(int userId, UpdateRecruitment request);

        Task<TResponse<bool>> Delete(int userId, int id);
    }

    public class RecruitmentService : BaseService,
                                    IRecruitmentService
    {
        readonly IRoleService _roleService;

        public RecruitmentService(IDapperReadOnlyRepository readOnlyRepository,
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
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.RECRUITMENT_DELETE,
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

        #region IRecruitmentService Members

        public async Task<TResponse<RecruitmentSimpleModel>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<RecruitmentSimpleModel>(SqlQuery.RECRUITMENT_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<RecruitmentSimpleModel>(result.Message);
                }

                return await Fail<RecruitmentSimpleModel>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<RecruitmentSimpleModel>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetRecruitmentPagingResponse>>> GetPaging(int userId, GetRecruitmentPagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetRecruitmentPagingResponse, int>(string.Format(SqlQuery.RECRUITMENT_GET_BY_PAGING,request.ColumnOrder,request.SortDir.ToUpper()),
                                                                                                              new
                                                                                                              {
                                                                                                                  Name = $"%{request.Name}%",
                                                                                                                  FromDate = $"{request.FromDate}",
                                                                                                                  ToDate = $"{request.ToDate}",
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetRecruitmentPagingResponse>(request.Page,
                                                                                  request.PageSize,
                                                                                  result.Data.Item2,
                                                                                  result.Data.Item1));
                    }

                    return await Fail<PageResult<GetRecruitmentPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetRecruitmentPagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetRecruitmentPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertRecruitment request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    string code = $"TD{DateTime.Today.ToString("yyMMdd")}001";
                    var rs = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.RECRUITMENT_CURRENT_COUNT, new { CurrentDate = $"{DateTime.Today.ToString("yyyy-MM-dd")}" });
                    if (rs.IsSuccess)
                    {
                        code = $"TD{DateTime.Today.ToString("yyMMdd")}{(rs.Data+1).ToString("D3")}";
                    }
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.RECRUITMENT_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,                                                                                                                  
                                                                                                                  Code = code,
                                                                                                                  request.Description,
                                                                                                                  request.Content,
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

        public async Task<TResponse<bool>> Update(int userId, UpdateRecruitment request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.RECRUITMENT_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.RecruitmentStatusId,
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

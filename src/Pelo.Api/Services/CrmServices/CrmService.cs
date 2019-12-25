using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Crm;
using Pelo.Common.Dtos.User;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.CrmServices
{
    public interface ICrmService
    {
        /// <summary>
        ///     Kiểm tra xem user đó có quyền xem hết các CRM hay không.
        ///     Nếu ko thì gán lại UserCreated = userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<TResponse<PageResult<GetCrmPagingResponse>>> GetPaging(int userId,
                                                                    GetCrmPagingRequest request);
    }

    public class CrmService : BaseService,
                              ICrmService
    {
        private readonly IAppConfigService _appConfigService;

        private readonly IRoleService _roleService;

        public CrmService(IDapperReadOnlyRepository readOnlyRepository,
                          IDapperWriteRepository writeRepository,
                          IHttpContextAccessor context,
                          IRoleService roleService,
                          IAppConfigService appConfigService) : base(readOnlyRepository,
                                                                     writeRepository,
                                                                     context)
        {
            _roleService = roleService;
            _appConfigService = appConfigService;
        }

        #region ICrmService Members

        /// <summary>
        ///     Kiểm tra xem user đó có quyền xem hết các CRM hay không.
        ///     Nếu ko thì gán lại UserCreated = userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponse<PageResult<GetCrmPagingResponse>>> GetPaging(int userId,
                                                                                 GetCrmPagingRequest request)
        {
            try
            {
                var canGetPaging = await CanGetPaging(userId);
                if(canGetPaging.IsSuccess)
                {
                    bool canGetAll = false;

                    var canGetAllCrm = await _appConfigService.GetByName("DefaultCRMAcceptRoles");
                    if(canGetAllCrm.IsSuccess)
                    {
                        var defaultRoles = canGetAllCrm.Message.Split(" ");
                        var currentRole = await _roleService.GetNameByUserId(userId);
                        if(currentRole.IsSuccess
                           && !string.IsNullOrEmpty(currentRole.Data)
                           && defaultRoles.Contains(currentRole.Data))
                        {
                            canGetAll = true;
                        }
                    }

                    if(!canGetAll)
                    {
                        request.UserCreatedId = userId;
                    }

                    var result = new TResponse<(IEnumerable<GetCrmPagingResponse>, int)>();

                    if(request.UserCareId == 0)
                    {
                        result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCrmPagingResponse, int>(SqlQuery.CRM_GET_BY_PAGING,
                                                                                                          new
                                                                                                          {
                                                                                                                  Customercode = $"%{request.CustomerCode}%",
                                                                                                                  CustomerName = $"%{request.CustomerName}%",
                                                                                                                  CustomerPhone = $"%{request.CustomerPhone}%",
                                                                                                                  CustomerAddress = $"%{request.CustomerAddress}%",
                                                                                                                  request.ProvinceId,
                                                                                                                  request.DistrictId,
                                                                                                                  request.WardId,
                                                                                                                  request.CustomerGroupId,
                                                                                                                  request.CustomerVipId,
                                                                                                                  request.CustomerSourceId,
                                                                                                                  Code = $"%{request.Code}%",
                                                                                                                  Need = $"%{request.Need}%",
                                                                                                                  request.CrmPriorityId,
                                                                                                                  request.CrmStatusId,
                                                                                                                  request.CrmTypeId,
                                                                                                                  request.ProductGroupId,
                                                                                                                  request.Visit,
                                                                                                                  FromDate = request.FromDate ?? new DateTime(1990,
                                                                                                                                                              1,
                                                                                                                                                              1),
                                                                                                                  ToDate = request.ToDate ?? new DateTime(2200,
                                                                                                                                                          1,
                                                                                                                                                          1),
                                                                                                                  request.UserCreatedId,
                                                                                                                  DateCreated = request.DateCreated ?? new DateTime(2000,
                                                                                                                                                                    1,
                                                                                                                                                                    1,
                                                                                                                                                                    0,
                                                                                                                                                                    0,
                                                                                                                                                                    0),
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                          });
                    }
                    else
                    {
                        result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCrmPagingResponse, int>(SqlQuery.CRM_GET_BY_PAGING_2,
                                                                                                          new
                                                                                                          {
                                                                                                                  Customercode = $"%{request.CustomerCode}%",
                                                                                                                  CustomerName = $"%{request.CustomerName}%",
                                                                                                                  CustomerPhone = $"%{request.CustomerPhone}%",
                                                                                                                  CustomerAddress = $"%{request.CustomerAddress}%",
                                                                                                                  request.ProvinceId,
                                                                                                                  request.DistrictId,
                                                                                                                  request.WardId,
                                                                                                                  request.CustomerGroupId,
                                                                                                                  request.CustomerVipId,
                                                                                                                  request.CustomerSourceId,
                                                                                                                  Code = $"%{request.Code}%",
                                                                                                                  Need = $"%{request.Need}%",
                                                                                                                  request.CrmPriorityId,
                                                                                                                  request.CrmStatusId,
                                                                                                                  request.CrmTypeId,
                                                                                                                  request.ProductGroupId,
                                                                                                                  request.Visit,
                                                                                                                  request.FromDate,
                                                                                                                  request.ToDate,
                                                                                                                  ContactDate = request.DateCreated,
                                                                                                                  request.UserCreatedId,
                                                                                                                  request.DateCreated,
                                                                                                                  request.UserCareId,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                          });
                    }

                    if(result.IsSuccess)
                    {
                        foreach (var crm in result.Data.Item1)
                        {
                            crm.UserCares = new List<UserDisplaySimpleModel>();
                            var crmUserCare = await ReadOnlyRepository.Query<UserDisplaySimpleModel>(SqlQuery.CRM_USER_GET_BY_CRM_ID,
                                                                                                     new
                                                                                                     {
                                                                                                             CrmId = crm.Id
                                                                                                     });
                            if (crmUserCare.IsSuccess && crmUserCare.Data != null)
                            {
                                crm.UserCares.AddRange(crmUserCare.Data);
                            }
                        }

                        return await Ok(new PageResult<GetCrmPagingResponse>(request.Page,
                                                                             request.PageSize,
                                                                             result.Data.Item2,
                                                                             result.Data.Item1));
                    }

                    return await Fail<PageResult<GetCrmPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetCrmPagingResponse>>(canGetPaging.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCrmPagingResponse>>(exception);
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
    }
}

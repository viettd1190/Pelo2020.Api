using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Crm;
using Pelo.Common.Dtos.User;
using Pelo.Common.Enums;
using Pelo.Common.Events.Crm;
using Pelo.Common.Extensions;
using Pelo.Common.Kafka;
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

        Task<TResponse<PageResult<GetCrmPagingResponse>>> KhachChuaXuLyTrongNgay(int userId,
                                                                                 GetPagingModel request);

        Task<TResponse<PageResult<GetCrmPagingResponse>>> KhachToiHenCanChamSoc(int userId,
                                                                                GetPagingModel request);

        Task<TResponse<PageResult<GetCrmPagingResponse>>> KhachQuaHenChamSoc(int userId,
                                                                             GetPagingModel request);

        Task<TResponse<PageResult<GetCrmPagingResponse>>> KhachToiHenNgayMai(int userId,
                                                                             GetPagingModel request);

        /// <summary>
        ///     Kiểm tra xem user đó có quyền add CRM hay không.
        ///     Nếu ko thì gán lại UserCreated = userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<TResponse<bool>> InsertCrm(int userId,
                                        InsertCrmRequest request);

        /// <summary>
        ///     Lấy danh sách CRM theo customer id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<TResponse<PageResult<GetCrmPagingResponse>>> GetByCustomerId(int userId,
                                                                          int customerId,
                                                                          int page,
                                                                          int pageSize);

        Task<TResponse<bool>> UpdateCrm(int userId,
                                        UpdateCrmRequest request);

        Task<TResponse<GetCrmModelReponse>> GetById(int userId,
                                                    int id);

        Task<TResponse<bool>> Comment(int userId,
                                      CommentCrmRequest comment);

        Task<TResponse<IEnumerable<CrmLogResponse>>> GetCrmLogs(int userId,
                                                                int crmId);
    }

    public class CrmService : BaseService,
                              ICrmService
    {
        private readonly IAppConfigService _appConfigService;

        private readonly IBusPublisher _busPublisher;

        private readonly IRoleService _roleService;

        private readonly IUserService _userService;

        public CrmService(IDapperReadOnlyRepository readOnlyRepository,
                          IDapperWriteRepository writeRepository,
                          IHttpContextAccessor context,
                          IRoleService roleService,
                          IAppConfigService appConfigService,
                          IUserService userService,
                          IBusPublisher busPublisher) : base(readOnlyRepository,
                                                             writeRepository,
                                                             context)
        {
            _roleService = roleService;
            _appConfigService = appConfigService;
            _userService = userService;
            _busPublisher = busPublisher;
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
                        var defaultRoles = canGetAllCrm.Data.Split(" ");
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
                            var crmUserCare = await ReadOnlyRepository.Query<UserDisplaySimpleModel>(SqlQuery.CRM_USER_CARE_GET_BY_CRM_ID,
                                                                                                     new
                                                                                                     {
                                                                                                             CrmId = crm.Id
                                                                                                     });
                            if(crmUserCare.IsSuccess
                               && crmUserCare.Data != null)
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

        public async Task<TResponse<PageResult<GetCrmPagingResponse>>> KhachChuaXuLyTrongNgay(int userId,
                                                                                              GetPagingModel request)
        {
            try
            {
                var now = DateTime.Now;

                var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCrmPagingResponse, int>(SqlQuery.CRM_KHACH_CHUA_XU_LY,
                                                                                                      new
                                                                                                      {
                                                                                                              CrmStatusId = 2,
                                                                                                              FromDate = new DateTime(now.Year,
                                                                                                                                      now.Month,
                                                                                                                                      now.Day,
                                                                                                                                      0,
                                                                                                                                      0,
                                                                                                                                      0),
                                                                                                              ToDate = new DateTime(now.Year,
                                                                                                                                    now.Month,
                                                                                                                                    now.Day,
                                                                                                                                    23,
                                                                                                                                    59,
                                                                                                                                    59),
                                                                                                              UserCareId = userId,
                                                                                                              Skip = (request.Page - 1) * request.PageSize,
                                                                                                              Take = request.PageSize
                                                                                                      });

                if(result.IsSuccess)
                {
                    foreach (var crm in result.Data.Item1)
                    {
                        crm.UserCares = new List<UserDisplaySimpleModel>();
                        var crmUserCare = await ReadOnlyRepository.Query<UserDisplaySimpleModel>(SqlQuery.CRM_USER_CARE_GET_BY_CRM_ID,
                                                                                                 new
                                                                                                 {
                                                                                                         CrmId = crm.Id
                                                                                                 });
                        if(crmUserCare.IsSuccess
                           && crmUserCare.Data != null)
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
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCrmPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetCrmPagingResponse>>> KhachToiHenCanChamSoc(int userId,
                                                                                             GetPagingModel request)
        {
            try
            {
                var now = DateTime.Now;

                var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCrmPagingResponse, int>(SqlQuery.CRM_KHACH_TOI_HEN_CAN_CHAM_SOC,
                                                                                                      new
                                                                                                      {
                                                                                                              FromDate = new DateTime(now.Year,
                                                                                                                                      now.Month,
                                                                                                                                      now.Day,
                                                                                                                                      0,
                                                                                                                                      0,
                                                                                                                                      0),
                                                                                                              ToDate = new DateTime(now.Year,
                                                                                                                                    now.Month,
                                                                                                                                    now.Day,
                                                                                                                                    23,
                                                                                                                                    59,
                                                                                                                                    59),
                                                                                                              UserCareId = userId,
                                                                                                              Skip = (request.Page - 1) * request.PageSize,
                                                                                                              Take = request.PageSize
                                                                                                      });

                if(result.IsSuccess)
                {
                    foreach (var crm in result.Data.Item1)
                    {
                        crm.UserCares = new List<UserDisplaySimpleModel>();
                        var crmUserCare = await ReadOnlyRepository.Query<UserDisplaySimpleModel>(SqlQuery.CRM_USER_CARE_GET_BY_CRM_ID,
                                                                                                 new
                                                                                                 {
                                                                                                         CrmId = crm.Id
                                                                                                 });
                        if(crmUserCare.IsSuccess
                           && crmUserCare.Data != null)
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
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCrmPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetCrmPagingResponse>>> KhachQuaHenChamSoc(int userId,
                                                                                          GetPagingModel request)
        {
            try
            {
                var now = DateTime.Now;

                var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCrmPagingResponse, int>(SqlQuery.CRM_KHACH_QUA_HEN_CHAM_SOC,
                                                                                                      new
                                                                                                      {
                                                                                                              FromDate = default(DateTime?),
                                                                                                              ToDate = new DateTime(now.Year,
                                                                                                                                    now.Month,
                                                                                                                                    now.Day,
                                                                                                                                    0,
                                                                                                                                    0,
                                                                                                                                    0),
                                                                                                              UserCareId = userId,
                                                                                                              Skip = (request.Page - 1) * request.PageSize,
                                                                                                              Take = request.PageSize
                                                                                                      });

                if(result.IsSuccess)
                {
                    foreach (var crm in result.Data.Item1)
                    {
                        crm.UserCares = new List<UserDisplaySimpleModel>();
                        var crmUserCare = await ReadOnlyRepository.Query<UserDisplaySimpleModel>(SqlQuery.CRM_USER_CARE_GET_BY_CRM_ID,
                                                                                                 new
                                                                                                 {
                                                                                                         CrmId = crm.Id
                                                                                                 });
                        if(crmUserCare.IsSuccess
                           && crmUserCare.Data != null)
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
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCrmPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetCrmPagingResponse>>> KhachToiHenNgayMai(int userId,
                                                                                          GetPagingModel request)
        {
            try
            {
                var tomorrow = DateTime.Now.AddDays(1);

                var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCrmPagingResponse, int>(SqlQuery.CRM_KHACH_QUA_HEN_CHAM_SOC,
                                                                                                      new
                                                                                                      {
                                                                                                              FromDate = new DateTime(tomorrow.Year,
                                                                                                                                      tomorrow.Month,
                                                                                                                                      tomorrow.Day,
                                                                                                                                      0,
                                                                                                                                      0,
                                                                                                                                      0),
                                                                                                              ToDate = new DateTime(tomorrow.Year,
                                                                                                                                    tomorrow.Month,
                                                                                                                                    tomorrow.Day,
                                                                                                                                    23,
                                                                                                                                    59,
                                                                                                                                    59),
                                                                                                              UserCareId = userId,
                                                                                                              Skip = (request.Page - 1) * request.PageSize,
                                                                                                              Take = request.PageSize
                                                                                                      });

                if(result.IsSuccess)
                {
                    foreach (var crm in result.Data.Item1)
                    {
                        crm.UserCares = new List<UserDisplaySimpleModel>();
                        var crmUserCare = await ReadOnlyRepository.Query<UserDisplaySimpleModel>(SqlQuery.CRM_USER_CARE_GET_BY_CRM_ID,
                                                                                                 new
                                                                                                 {
                                                                                                         CrmId = crm.Id
                                                                                                 });
                        if(crmUserCare.IsSuccess
                           && crmUserCare.Data != null)
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
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCrmPagingResponse>>(exception);
            }
        }

        /// <summary>
        ///     Kiểm tra xem user đó có quyền xem hết các CRM hay không.
        ///     Nếu ko thì gán lại UserCreated = userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponse<bool>> InsertCrm(int userId,
                                                     InsertCrmRequest request)
        {
            try
            {
                var canGetPaging = await CanGetPaging(userId);
                if(canGetPaging.IsSuccess)
                {
                    //bool canGetAll = false;

                    //var canGetAllCrm = await _appConfigService.GetByName("DefaultCRMAcceptRoles");
                    //if (canGetAllCrm.IsSuccess)
                    //{
                    //    var defaultRoles = canGetAllCrm.Data.Split(" ");
                    //    var currentRole = await _roleService.GetNameByUserId(userId);
                    //    if (currentRole.IsSuccess
                    //       && !string.IsNullOrEmpty(currentRole.Data)
                    //       && defaultRoles.Contains(currentRole.Data))
                    //    {
                    //        canGetAll = true;
                    //    }
                    //}

                    //if (!canGetAll)
                    //{
                    //    request.UserCreatedId = userId;
                    //}

                    var crmCodeResponse = await BuildCrmCode(DateTime.Now);
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.CRM_INSERT,
                                                                               new
                                                                               {
                                                                                       request.CustomerId,
                                                                                       request.CrmStatusId,
                                                                                       request.ContactDate,
                                                                                       request.ProductGroupId,
                                                                                       request.Need,
                                                                                       request.Description,
                                                                                       request.CustomerSourceId,
                                                                                       request.CrmPriorityId,
                                                                                       request.CrmTypeId,
                                                                                       Code = crmCodeResponse.Data,
                                                                                       request.Visit,
                                                                                       UserCreated = userId,
                                                                                       DateCreated = DateTime.Now,
                                                                                       UserUpdated = userId,
                                                                                       DateUpdated = DateTime.Now
                                                                               });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            var crmId = result.Data;

                            #region 3. Thêm người liên quan

                            #region 3.1. Kiểm tra xem người tạo có trong danh sách người liên quan không, nếu không có thì thêm vào

                            if(request.UserIds == null)
                            {
                                request.UserIds = new List<int>();
                            }

                            if(!request.UserIds.Any())
                            {
                                request.UserIds.Add(userId);
                            }

                            if(!request.UserIds.Contains(userId))
                            {
                                request.UserIds.Add(userId);
                            }

                            #endregion

                            if(request.UserIds != null)
                            {
                                if(request.UserIds.Any())
                                {
                                    foreach (var user in request.UserIds)
                                    {
                                        await WriteRepository.ExecuteAsync(SqlQuery.CRM_USER_INSERT,
                                                                           new
                                                                           {
                                                                                   CrmId = crmId,
                                                                                   UserId = user,
                                                                                   Type = 0,
                                                                                   UserCreated = userId,
                                                                                   DateCreated = DateTime.Now,
                                                                                   UserUpdated = userId,
                                                                                   DateUpdated = DateTime.Now
                                                                           });
                                    }
                                }
                            }

                            #endregion

                            #region 4. Thêm thông báo

                            await _busPublisher.SendEventAsync(new CrmInsertSuccessEvent
                                                               {
                                                                       Id = crmId,
                                                                       Code = crmCodeResponse.Data,
                                                                       CrmStatusId = request.CrmStatusId,
                                                                       UserId = userId,
                                                                       UserIds = request.UserIds
                                                               });

                            #endregion

                            return await Ok(true);
                        }

                        return await Fail<bool>("Error");
                    }
                }
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }

            return await Fail<bool>("Error");
        }

        public async Task<TResponse<PageResult<GetCrmPagingResponse>>> GetByCustomerId(int userId,
                                                                                       int customerId,
                                                                                       int page,
                                                                                       int pageSize)
        {
            try
            {
                var result = new TResponse<(IEnumerable<GetCrmPagingResponse>, int)>();

                bool canGetAll = false;

                var canGetAllCrm = await _appConfigService.GetByName("DefaultCRMAcceptRoles");
                if(canGetAllCrm.IsSuccess)
                {
                    var defaultRoles = canGetAllCrm.Data.Split(" ");
                    var currentRole = await _roleService.GetNameByUserId(userId);
                    if(currentRole.IsSuccess
                       && !string.IsNullOrEmpty(currentRole.Data)
                       && defaultRoles.Contains(currentRole.Data))
                    {
                        canGetAll = true;
                    }
                }

                if(canGetAll)
                {
                    result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCrmPagingResponse, int>(SqlQuery.CRM_GET_BY_CUSTOMER_ID,
                                                                                                      new
                                                                                                      {
                                                                                                              CustomerId = customerId,
                                                                                                              Skip = (page - 1) * pageSize,
                                                                                                              Take = pageSize
                                                                                                      });
                }
                else
                {
                    result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCrmPagingResponse, int>(SqlQuery.CRM_GET_BY_CUSTOMER_ID_2,
                                                                                                      new
                                                                                                      {
                                                                                                              CustomerId = customerId,
                                                                                                              UserCareId = userId,
                                                                                                              Skip = (page - 1) * pageSize,
                                                                                                              Take = pageSize
                                                                                                      });
                }

                if(result.IsSuccess)
                {
                    foreach (var crm in result.Data.Item1)
                    {
                        crm.UserCares = new List<UserDisplaySimpleModel>();
                        var crmUserCare = await ReadOnlyRepository.Query<UserDisplaySimpleModel>(SqlQuery.CRM_USER_CARE_GET_BY_CRM_ID,
                                                                                                 new
                                                                                                 {
                                                                                                         CrmId = crm.Id
                                                                                                 });
                        if(crmUserCare.IsSuccess
                           && crmUserCare.Data != null)
                        {
                            crm.UserCares.AddRange(crmUserCare.Data);
                        }
                    }

                    return await Ok(new PageResult<GetCrmPagingResponse>(page,
                                                                         pageSize,
                                                                         result.Data.Item2,
                                                                         result.Data.Item1));
                }

                return await Fail<PageResult<GetCrmPagingResponse>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCrmPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> UpdateCrm(int userId,
                                                     UpdateCrmRequest request)
        {
            try
            {
                var oldInformation = await CanUpdate(userId,
                                                     request);
                if(oldInformation.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.CRM_UPDATE,
                                                                    new
                                                                    {
                                                                            request.Id,
                                                                            request.ContactDate,
                                                                            request.ProductGroupId,
                                                                            request.CrmTypeId,
                                                                            request.Need,
                                                                            request.Description,
                                                                            request.CustomerSourceId,
                                                                            request.CrmPriorityId,
                                                                            request.Visit,
                                                                            UserUpdated = userId,
                                                                            DateUpdated = DateTime.Now
                                                                    });
                    if(result.IsSuccess)
                    {
                        if(result.Data > 0)
                        {
                            if(oldInformation.Data.CrmPriorityId != request.CrmPriorityId)
                            {
                            }

                            if(oldInformation.Data.CrmTypeId != request.CrmTypeId)
                            {
                            }

                            if(oldInformation.Data.Visit != request.Visit)
                            {
                            }

                            if(oldInformation.Data.Need != request.Need)
                            {
                            }

                            if(oldInformation.Data.ProductGroupId != request.ProductGroupId)
                            {
                            }

                            if(oldInformation.Data.CustomerSourceId != request.CustomerSourceId)
                            {
                            }

                            if(oldInformation.Data.ContactDate != request.ContactDate)
                            {
                            }

                            if(oldInformation.Data.Description != request.Description)
                            {
                            }

                            var crmId = result.Data;
                            var crmUser = await ReadOnlyRepository.QueryAsync<CrmUserResponse>(SqlQuery.GET_CRM_USER_BY_CRMID,
                                                                                               new
                                                                                               {
                                                                                                       CrmId = crmId
                                                                                               });
                            if(request.UserIds == null)
                            {
                                request.UserIds = new List<int>();
                            }

                            if(crmUser.Data.Any()
                               || request.UserIds.Any())
                            {
                                if(crmUser.Data.Any()
                                   && (request.UserIds == null || !request.UserIds.Any()))
                                {
                                }
                                else if(request.UserIds.Any()
                                        && (crmUser.Data == null || !crmUser.Data.Any()))
                                {
                                }
                            }

                            if(crmUser != null)
                            {
                                if(request.UserIds.Any())
                                {
                                    CrmUserResponse[] crmUserResponse = crmUser.Data.Where(c => c.CrmId == request.Id && !request.UserIds.Contains(c.UserId))
                                                                               .ToArray();
                                    foreach (var item in crmUserResponse)
                                    {
                                        await WriteRepository.ExecuteAsync(SqlQuery.CRM_USER_DELETE,
                                                                           new
                                                                           {
                                                                                   CrmId = request.Id,
                                                                                   UserId = item.Id
                                                                           });
                                    }

                                    foreach (var user in request.UserIds)
                                    {
                                        var rs = await WriteRepository.ExecuteAsync(SqlQuery.CRM_USER_INSERT,
                                                                                    new
                                                                                    {
                                                                                            CrmId = request.Id,
                                                                                            UserId = user,
                                                                                            Type = 0,
                                                                                            UserUpdated = userId,
                                                                                            UserCreated = userId,
                                                                                            DateUpdated = DateTime.Now,
                                                                                            DateCreated = DateTime.Now
                                                                                    });
                                    }
                                }
                                else
                                {
                                    if(crmUser.IsSuccess)
                                    {
                                        foreach (var item in crmUser.Data)
                                        {
                                            var rs = await WriteRepository.ExecuteAsync(SqlQuery.CRM_USER_DELETE,
                                                                                        new
                                                                                        {
                                                                                                CrmId = request.Id,
                                                                                                UserId = item.Id
                                                                                        });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var user in request.UserIds)
                                {
                                    var rs = await WriteRepository.ExecuteAsync(SqlQuery.CRM_USER_INSERT,
                                                                                new
                                                                                {
                                                                                        CrmId = request.Id,
                                                                                        UserId = user,
                                                                                        Type = 0,
                                                                                        UserUpdated = userId,
                                                                                        UserCreated = userId,
                                                                                        DateUpdated = DateTime.Now,
                                                                                        DateCreated = DateTime.Now
                                                                                });
                                }
                            }

                            return await Ok(true);
                        }

                        return await Fail<bool>("Can not execute CRM_UPDATE");
                    }

                    return await Fail<bool>(result.Message);
                }

                return await Fail<bool>(oldInformation.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        public async Task<TResponse<GetCrmModelReponse>> GetById(int userId,
                                                                 int id)
        {
            try
            {
                var canGetPaging = await CanGetPaging(userId);
                if(canGetPaging.IsSuccess)
                {
                    var result = new TResponse<GetCrmModelReponse>();
                    var canGetAllCrm = await _appConfigService.GetByName("DefaultCRMAcceptRoles");
                    if(canGetAllCrm.IsSuccess)
                    {
                        result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetCrmModelReponse>(SqlQuery.GET_CRM_BY_ID,
                                                                                                       new
                                                                                                       {
                                                                                                               Id = id
                                                                                                       });
                        if(result.IsSuccess)
                        {
                            if(result.Data != null)
                            {
                                result.Data.UserCares = new List<UserDisplaySimpleModel>();
                                var crmUserCare = await ReadOnlyRepository.Query<UserDisplaySimpleModel>(SqlQuery.CRM_USER_CARE_GET_BY_CRM_ID,
                                                                                                         new
                                                                                                         {
                                                                                                                 CrmId = id
                                                                                                         });
                                if(crmUserCare.IsSuccess
                                   && crmUserCare.Data != null)
                                {
                                    result.Data.UserCares.AddRange(crmUserCare.Data);
                                }

                                return await Ok(result.Data);
                            }
                        }

                        return await Fail<GetCrmModelReponse>(ErrorEnum.CRM_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<GetCrmModelReponse>(ErrorEnum.USER_DO_HAVE_NOT_PERMISSON_VIEW_THIS_CRM.GetStringValue());
                }

                return await Fail<GetCrmModelReponse>(canGetPaging.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetCrmModelReponse>(exception);
            }
        }

        public async Task<TResponse<bool>> Comment(int userId,
                                                   CommentCrmRequest request)
        {
            try
            {
                var crm = await ReadOnlyRepository.QueryFirstOrDefaultAsync<CrmDetailResponse>(SqlQuery.GET_CRM_BY_ID,
                                                                                               new
                                                                                               {
                                                                                                       request.Id
                                                                                               });
                if(crm.IsSuccess)
                {
                    if(crm.Data != null)
                    {
                        var rs = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.CRM_INSERT_COMMENT,
                                                                               new
                                                                               {
                                                                                       CrmId = request.Id,
                                                                                       request.Comment,
                                                                                       UserId = userId
                                                                               });
                        if(rs.IsSuccess)
                        {
                            if(request.Files != null
                               && request.Files.Any())
                            {
                                var crmLogId = rs.Data;

                                var path = "\\wwwroot\\Attachments";

                                if(!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }

                                foreach (var file in request.Files)
                                {
                                    var newFileName = RenameFile(file.FileName);

                                    using (FileStream fileStream = File.Create(Path.Combine(path,
                                                                                            newFileName) + Path.GetExtension(file.FileName)))
                                    {
                                        file.CopyTo(fileStream);
                                        fileStream.Flush();

                                        var result = await WriteRepository.ExecuteAsync(SqlQuery.CRM_LOG_ATTACHMENT_INSERT,
                                                                                        new
                                                                                        {
                                                                                                CrmLogId = crmLogId,
                                                                                                Attachment = $"{newFileName}{Path.GetExtension(file.FileName)}",
                                                                                                AttachmentName = file.FileName,
                                                                                                UserCreated = userId,
                                                                                                UserUpdated = userId
                                                                                        });
                                    }
                                }

                                return await Ok(true);
                            }

                            return await Ok(true);
                        }
                    }
                }

                return await Fail<bool>(ErrorEnum.CRM_HAS_NOT_EXIST.GetStringValue());
            }
            catch (Exception)
            {
                return await Fail<bool>(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue());
            }
        }

        public async Task<TResponse<IEnumerable<CrmLogResponse>>> GetCrmLogs(int userId,
                                                                             int crmId)
        {
            try
            {
                var result = await ReadOnlyRepository.QueryAsync<CrmLogResponse>(SqlQuery.CRM_GET_LOGS,
                                                                                 new
                                                                                 {
                                                                                         CrmId = crmId
                                                                                 });
                if(result.IsSuccess)
                {
                    foreach (var log in result.Data)
                    {
                        var user = await ReadOnlyRepository.QueryFirstOrDefaultAsync<UserInLog>(SqlQuery.GET_USER_IN_LOG,
                                                                                                new
                                                                                                {
                                                                                                        Id = log.UserId
                                                                                                });
                        if(user.IsSuccess
                           && user.Data != null)
                        {
                            log.User = user.Data;
                        }

                        var oldCrmStatus = await ReadOnlyRepository.QueryFirstOrDefaultAsync<CrmStatusInLog>(SqlQuery.GET_CRM_STATUS_IN_LOG,
                                                                                                             new
                                                                                                             {
                                                                                                                     Id = log.OldCrmStatusId
                                                                                                             });
                        if(oldCrmStatus.IsSuccess)
                        {
                            log.OldCrmStatus = oldCrmStatus.Data;
                        }

                        var crmStatus = await ReadOnlyRepository.QueryFirstOrDefaultAsync<CrmStatusInLog>(SqlQuery.GET_CRM_STATUS_IN_LOG,
                                                                                                          new
                                                                                                          {
                                                                                                                  Id = log.CrmStatusId
                                                                                                          });
                        if(crmStatus.IsSuccess)
                        {
                            log.CrmStatus = crmStatus.Data;
                        }

                        var attachments = await ReadOnlyRepository.QueryAsync<CrmLogAttachment>(SqlQuery.GET_CRM_ATTACHMENT_IN_LOG,
                                                                                                new
                                                                                                {
                                                                                                        CrmLogId = log.Id
                                                                                                });

                        if(attachments.IsSuccess)
                        {
                            log.Attachments = attachments.Data.ToList();
                        }
                    }

                    return await Ok(result.Data);
                }

                return await Fail<IEnumerable<CrmLogResponse>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<CrmLogResponse>>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                                             "GetCrmLog"));
            }
        }

        #endregion

        private string RenameFile(string fileName)
        {
            var newName = Guid.NewGuid()
                              .ToString()
                              .Replace("-",
                                       string.Empty);
            return newName;
        }

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

        private async Task<TResponse<bool>> Notify(int userId,
                                                   List<int> userReceiveIds,
                                                   string title,
                                                   string message,
                                                   string code,
                                                   int crmId)
        {
            return await Ok(true);
            //try
            //{
            //    var disableNotifications = await _notificationService.DisableNotification();
            //    if (disableNotifications.IsSuccess)
            //    {
            //        foreach (var userReceiveId in userReceiveIds)
            //        {
            //            InsertNotificationModel notification = new InsertNotificationModel
            //            {
            //                UserId = userId,
            //                UserReceiveId = userReceiveId,
            //                Title = $"{title} <b>{code}</b>",
            //                Message = message,
            //                Action = $"/Crm/Detail/{crmId}",
            //                Type = (int)NotificationTypeEnum.CRM,
            //                Status = userReceiveId == userId
            //                                                                            ? 1
            //                                                                            : 0
            //            };
            //            var result = await _notificationService.Add(notification);
            //            if (result.IsSuccess)
            //            {
            //                KafkaHelper.PublishMessage(Constants.KAFKA_URL_SERVER,
            //                                           Constants.TOPIC_UPDATE_NOTIFICATION,
            //                                           new UpdateNotificationKafkaMessage
            //                                           {
            //                                               UserId = userReceiveId,
            //                                               Status = UpdateUserNotificationEnum.INSERT
            //                                           });
            //            }
            //        }
            //    }

            //    return await Fail<bool>(disableNotifications.Message);
            //}
            //catch (Exception exception)
            //{
            //    return await Fail<bool>(exception);
            //}
        }

        private async Task<TResponse<string>> BuildCrmCode(DateTime date)
        {
            try
            {
                var crmPrefixResponse = await _appConfigService.GetByName("CRMPrefix");
                string crmPrefix = "CH";
                if(crmPrefixResponse.IsSuccess)
                {
                    crmPrefix = crmPrefixResponse.Data;
                }

                var code = $"{crmPrefix}{(date.Year % 100):00}{date.Month:00}{date.Day:00}";
                var countResponses = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CRM_COUNT_BY_DATE,
                                                                                            new
                                                                                            {
                                                                                                    Code = $"{code}%"
                                                                                            });
                if(countResponses.IsSuccess)
                {
                    return await Ok($"{code}{(countResponses.Data + 1):000}");
                }

                return await Fail<string>(countResponses.Message);
            }
            catch (Exception exception)
            {
                return await Fail<string>(exception);
            }
        }

        private async Task<TResponse<GetCrmModelReponse>> CanUpdate(int userId,
                                                                    UpdateCrmRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if(checkPermission.IsSuccess)
                {
                    if(request.Id == 0)
                    {
                        return await Fail<GetCrmModelReponse>(ErrorEnum.CRM_HAS_NOT_EXIST.GetStringValue());
                    }

                    if(request.ProductGroupId == 0)
                    {
                        return await Fail<GetCrmModelReponse>(ErrorEnum.PRODUCT_GROUP_HAS_NOT_EXIST.GetStringValue());
                    }

                    if(request.CrmPriorityId == 0)
                    {
                        return await Fail<GetCrmModelReponse>(ErrorEnum.CRM_PRIORITY_HAS_NOT_EXIST.GetStringValue());
                    }

                    if(request.CrmTypeId == 0)
                    {
                        return await Fail<GetCrmModelReponse>(ErrorEnum.CRM_TYPE_HAS_NOT_EXIST.GetStringValue());
                    }

                    if(request.CustomerSourceId == 0)
                    {
                        return await Fail<GetCrmModelReponse>(ErrorEnum.CUSTOMER_SOURCE_HAS_NOT_EXIST.GetStringValue());
                    }

                    var checkIdInvalid = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetCrmModelReponse>(SqlQuery.GET_CRM_BY_ID,
                                                                                                               new
                                                                                                               {
                                                                                                                       request.Id
                                                                                                               });
                    if(checkIdInvalid.IsSuccess)
                    {
                        if(checkIdInvalid.Data != null)
                        {
                            return await Ok(checkIdInvalid.Data);
                        }

                        return await Fail<GetCrmModelReponse>(ErrorEnum.CRM_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<GetCrmModelReponse>(checkIdInvalid.Message);
                }

                return await Fail<GetCrmModelReponse>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetCrmModelReponse>(exception);
            }
        }
    }
}

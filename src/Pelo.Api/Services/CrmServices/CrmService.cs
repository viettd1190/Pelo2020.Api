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
    }

    public class CrmService : BaseService,
                              ICrmService
    {
        private readonly IAppConfigService _appConfigService;

        private readonly IRoleService _roleService;

        private readonly IUserService _userService;

        public CrmService(IDapperReadOnlyRepository readOnlyRepository,
                          IDapperWriteRepository writeRepository,
                          IHttpContextAccessor context,
                          IRoleService roleService,
                          IAppConfigService appConfigService,
                          IUserService userService) : base(readOnlyRepository,
                                                           writeRepository,
                                                           context)
        {
            _roleService = roleService;
            _appConfigService = appConfigService;
            _userService = userService;
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

                            #region 4.1. Kiểm tra xem user admin có trong danh sách người nhận thông báo không, nếu  không có thì thêm vào

                            var adminUserId = await _userService.GetByUsername("admin");
                            if(adminUserId.IsSuccess)
                            {
                                if(!request.UserIds.Contains(adminUserId.Data.Id))
                                {
                                    request.UserIds.Add(adminUserId.Data.Id);
                                }
                            }

                            #endregion

                            #region 4.2. hêm danh sách tài khoản mặc định nhận thông báo Crm

                            var notificationUserCrmsResponse = await _appConfigService.GetByName("NotificationCRMUsers");
                            if(notificationUserCrmsResponse != null)
                            {
                                if(!string.IsNullOrEmpty(notificationUserCrmsResponse.Data))
                                {
                                    var notificationUSerCrms = notificationUserCrmsResponse.Data.Split(' ');
                                    if(notificationUSerCrms.Any())
                                    {
                                        foreach (var notificationUSerCrm in notificationUSerCrms)
                                        {
                                            var notificationUser = await _userService.GetByUsername(notificationUSerCrm);
                                            if(notificationUser.IsSuccess)
                                            {
                                                if(!request.UserIds.Contains(notificationUser.Data.Id))
                                                {
                                                    request.UserIds.Add(notificationUser.Data.Id);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion

                            var resultNotification = await Notify(userId,
                                                                  request.UserIds,
                                                                  "đã thêm CRM mới",
                                                                  string.Empty,
                                                                  crmCodeResponse.Data,
                                                                  crmId);
                            if(!resultNotification.IsSuccess)
                            {
                                Log(resultNotification.Message);
                            }

                            #endregion

                            //#region 5. Thêm lịch sử chỉnh sửa Crm

                            //KafkaHelper.PublishMessage(Constants.KAFKA_URL_SERVER,
                            //                           Constants.TOPIC_AUDIT_TABLE,
                            //                           new AuditTableKafkaMessage<AuditCrm>
                            //                           {
                            //                               Table = AuditTableTypeEnum.CRM,
                            //                               LogType = LogTypeEnum.INSERT,
                            //                               Id = crmId,
                            //                               OldValue = null,
                            //                               Value = null,
                            //                               Comment = string.Empty,
                            //                               UserId = userId,
                            //                               Attachments = new Dictionary<string, string>()
                            //                           });

                            //#endregion

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
    }
}

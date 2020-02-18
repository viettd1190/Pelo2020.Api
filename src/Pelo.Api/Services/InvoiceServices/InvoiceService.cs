using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Invoice;
using Pelo.Common.Dtos.User;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.InvoiceServices
{
    public interface IInvoiceService
    {
        Task<TResponse<PageResult<GetInvoicePagingResponse>>> GetByCustomerId(int userId,
                                                                              int customerId,
                                                                              int page,
                                                                              int pageSize);

        Task<TResponse<PageResult<GetInvoicePagingResponse>>> GetPaging(int userId,
                                                                        GetInvoicePagingRequest request);

        Task<TResponse<bool>> Insert(int userId,
                                     InsertInvoiceRequest request);
    }

    public class InvoiceService : BaseService,
                                  IInvoiceService
    {
        private readonly IAppConfigService _appConfigService;

        private readonly IRoleService _roleService;

        private readonly IUserService _userService;

        private IProductService _productService;

        public InvoiceService(IDapperReadOnlyRepository readOnlyRepository,
                              IDapperWriteRepository writeRepository,
                              IHttpContextAccessor context,
                              IRoleService roleService,
                              IUserService userService,
                              IProductService productService,
                              IAppConfigService appConfigService) : base(readOnlyRepository,
                                                                         writeRepository,
                                                                         context)
        {
            _roleService = roleService;
            _appConfigService = appConfigService;
            _userService = userService;
            _productService = productService;
        }

        #region IInvoiceService Members

        public async Task<TResponse<PageResult<GetInvoicePagingResponse>>> GetByCustomerId(int userId,
                                                                                           int customerId,
                                                                                           int page,
                                                                                           int pageSize)
        {
            try
            {
                var result = new TResponse<(IEnumerable<GetInvoicePagingResponse>, int)>();

                bool canGetAll = false;

                var canGetAllCrm = await _appConfigService.GetByName("DefaultInvoiceAcceptRoles");
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
                    result = await ReadOnlyRepository.QueryMultipleLFAsync<GetInvoicePagingResponse, int>(SqlQuery.INVOICE_GET_BY_CUSTOMER_ID,
                                                                                                          new
                                                                                                          {
                                                                                                                  CustomerId = customerId,
                                                                                                                  Skip = (page - 1) * pageSize,
                                                                                                                  Take = pageSize
                                                                                                          });
                }
                else
                {
                    result = await ReadOnlyRepository.QueryMultipleLFAsync<GetInvoicePagingResponse, int>(SqlQuery.INVOICE_GET_BY_CUSTOMER_ID_2,
                                                                                                          new
                                                                                                          {
                                                                                                                  CustomerId = customerId,
                                                                                                                  UserId = userId,
                                                                                                                  Skip = (page - 1) * pageSize,
                                                                                                                  Take = pageSize
                                                                                                          });
                }

                if(result.IsSuccess)
                {
                    foreach (var invoice in result.Data.Item1)
                    {
                        invoice.UsersDelivery = new List<UserDisplaySimpleModel>();
                        var crmUserCare = await ReadOnlyRepository.QueryAsync<UserDisplaySimpleModel>(SqlQuery.INVOICE_USER_DELIVERY_GET_BY_INVOICE_ID,
                                                                                                      new
                                                                                                      {
                                                                                                              InvoiceId = invoice.Id
                                                                                                      });
                        if(crmUserCare.IsSuccess
                           && crmUserCare.Data != null)
                        {
                            invoice.UsersDelivery.AddRange(crmUserCare.Data);
                        }

                        invoice.Products = new List<ProductInInvoiceSimpleModel>();
                        var products = await ReadOnlyRepository.QueryAsync<ProductInInvoiceSimpleModel>(SqlQuery.PRODUCTS_IN_INVOICE_GET_BY_INVOICE_ID,
                                                                                                        new
                                                                                                        {
                                                                                                                InvoiceId = invoice.Id
                                                                                                        });
                        if(products.IsSuccess
                           && products.Data != null)
                        {
                            invoice.Products.AddRange(products.Data);
                        }
                    }

                    return await Ok(new PageResult<GetInvoicePagingResponse>(page,
                                                                             pageSize,
                                                                             result.Data.Item2,
                                                                             result.Data.Item1));
                }

                return await Fail<PageResult<GetInvoicePagingResponse>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetInvoicePagingResponse>>(exception);
            }
        }

        public async Task<TResponse<PageResult<GetInvoicePagingResponse>>> GetPaging(int userId,
                                                                                     GetInvoicePagingRequest request)
        {
            try
            {
                userId = 1;

                var canGetPaging = await CanGetPaging(userId);
                if(canGetPaging.IsSuccess)
                {
                    var isDefaultInvoiceRoles = await _userService.IsBelongDefaultInvoiceRole(userId);
                    if(!isDefaultInvoiceRoles.IsSuccess)
                    {
                        request.UserCreatedId = userId;
                        request.UserSellId = userId;
                        request.UserDeliveryId = userId;
                    }

                    var sqlQuery = await BuildSqlQueryGetPaging(userId,
                                                                request);

                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetInvoicePagingResponse, int>(sqlQuery,
                                                                                                              new
                                                                                                              {
                                                                                                                      Customercode = $"%{request.CustomerCode}%",
                                                                                                                      CustomerName = $"%{request.CustomerName}%",
                                                                                                                      CustomerPhone = $"%{request.CustomerPhone}%",
                                                                                                                      Code = $"%{request.Code}%",
                                                                                                                      request.BranchId,
                                                                                                                      request.InvoiceStatusId,
                                                                                                                      request.UserCreatedId,
                                                                                                                      request.UserSellId,
                                                                                                                      request.UserDeliveryId,
                                                                                                                      request.FromDate,
                                                                                                                      request.ToDate,
                                                                                                                      Skip = (request.Page - 1) * request.PageSize,
                                                                                                                      Take = request.PageSize
                                                                                                              });
                    if(result.IsSuccess)
                    {
                        foreach (var invoice in result.Data.Item1)
                        {
                            invoice.Products = new List<ProductInInvoiceSimpleModel>();
                            var products = await ReadOnlyRepository.QueryAsync<ProductInInvoiceSimpleModel>(SqlQuery.PRODUCTS_IN_INVOICE_GET_BY_INVOICE_ID,
                                                                                                            new
                                                                                                            {
                                                                                                                    InvoiceId = invoice.Id
                                                                                                            });
                            if(products.IsSuccess
                               && products.Data != null)
                            {
                                invoice.Products.AddRange(products.Data);
                            }

                            invoice.UsersDelivery = new List<UserDisplaySimpleModel>();
                            var deliveryUsers = await ReadOnlyRepository.QueryAsync<UserDisplaySimpleModel>(SqlQuery.INVOICE_USER_DELIVERY_GET_BY_INVOICE_ID,
                                                                                                            new
                                                                                                            {
                                                                                                                    InvoiceId = invoice.Id
                                                                                                            });
                            if(deliveryUsers.IsSuccess
                               && deliveryUsers.Data != null)
                            {
                                invoice.UsersDelivery.AddRange(deliveryUsers.Data);
                            }
                        }

                        return await Ok(new PageResult<GetInvoicePagingResponse>(request.Page,
                                                                                 request.PageSize,
                                                                                 result.Data.Item2,
                                                                                 result.Data.Item1));
                    }

                    return await Fail<PageResult<GetInvoicePagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetInvoicePagingResponse>>(canGetPaging.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetInvoicePagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId,
                                                  InsertInvoiceRequest request)
        {
            try
            {
                var canInsert = await CanInsert(userId);
                if(canInsert.IsSuccess)
                {
                    var invoiceCode = await BuildInvoiceCode(DateTime.Now);
                    var invoiceStatusId = await GetDefaultInvoiceStatus();
                    int totalAmount = 0;
                    if(request.Products!=null)
                    {
                        totalAmount = request.Products.Sum(c => c.Price * c.Quantity);
                    }

                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.INVOICE_INSERT,
                                                                               new
                                                                               {
                                                                                       Code = invoiceCode,
                                                                                       InvoiceStatusId = invoiceStatusId,
                                                                                       request.BranchId,
                                                                                       request.CustomerId,
                                                                                       request.PayMethodId,
                                                                                       Total = totalAmount,
                                                                                       request.Deposit,
                                                                                       DeliveryCode = 0,
                                                                                       request.Discount,
                                                                                       request.UserSellId,
                                                                                       request.DeliveryDate,
                                                                                       request.Description,
                                                                                       UserCreated = userId,
                                                                                       UserUpdated = userId
                                                                               });

                    if(result.IsSuccess)
                    {
                        var invoiceId = result.Data;

                        #region 1. Thêm sản phẩm

                        if(request.Products!=null)
                        {
                            foreach (var productInInvoiceRequest in request.Products)
                            {
                                var product = await _productService.GetSimpleById(productInInvoiceRequest.Id);
                                if (product != null)
                                {
                                    await WriteRepository.ExecuteAsync(SqlQuery.PRODUCT_IN_INVOICE_INSERT,
                                                                       new
                                                                       {
                                                                               ProductId = productInInvoiceRequest.Id,
                                                                               InvoiceId = invoiceId,
                                                                               Price = productInInvoiceRequest.Price,
                                                                               Quantity = productInInvoiceRequest.Quantity,
                                                                               ImportPrice = product.ImportPrice,
                                                                               ProductName = product.Name,
                                                                               Description = productInInvoiceRequest.Description,
                                                                               UserCreated = userId,
                                                                               UserUpdated = userId
                                                                       });
                                }
                            }
                        }

                        #endregion

                        #region 2. Thêm thông báo

                        List<int> receiveNotificationUserIds=new List<int>();

                        #region 2.1. Kiểm tra xem user admin có trong danh sách người nhận thông báo không, nếu  không có thì thêm vào

                        var adminUserId = await _userService.GetByUsername("admin");
                        if (adminUserId.IsSuccess)
                        {
                            receiveNotificationUserIds.Add(adminUserId.Data.Id);
                        }

                        #endregion

                        #region 4.2. Thêm danh sách tài khoản mặc định nhận thông báo Crm

                        var notificationUserInvoicesResponse = await _appConfigService.GetByName("NotificationInvoiceUsers");
                        if (notificationUserInvoicesResponse != null)
                        {
                            if (!string.IsNullOrEmpty(notificationUserInvoicesResponse.Data))
                            {
                                var notificationUSerCrms = notificationUserInvoicesResponse.Data.Split(' ');
                                if (notificationUSerCrms.Any())
                                {
                                    foreach (var notificationUSerCrm in notificationUSerCrms)
                                    {
                                        var notificationUser = await _userService.GetByUsername(notificationUSerCrm);
                                        if (notificationUser.IsSuccess)
                                        {
                                            if (!receiveNotificationUserIds.Contains(notificationUser.Data.Id))
                                            {
                                                receiveNotificationUserIds.Add(notificationUser.Data.Id);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        var resultNotification = await Notify(userId,
                                                              receiveNotificationUserIds,
                                                              "đã thêm đơn hàng mới",
                                                              string.Empty,
                                                              invoiceCode,
                                                              invoiceId);
                        //if (!resultNotification.IsSuccess)
                        //{
                        //    Log(resultNotification.Message);
                        //}

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

                    return await Fail<bool>(result.Message);
                }

                return await Fail<bool>(canInsert.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
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

        private async Task<TResponse<bool>> CanInsert(int userId)
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

        private async Task<string> BuildSqlQueryGetPaging(int userId,
                                                          GetInvoicePagingRequest request)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("DROP TABLE IF EXISTS #tmpInvoice; ");
            sqlBuilder.Append("SELECT i.Id INTO #tmpInvoice FROM dbo.Invoice i ");
            sqlBuilder.Append("LEFT JOIN dbo.Customer c ON c.Id = i.CustomerId ");
            sqlBuilder.Append("LEFT JOIN dbo.UserInInvoice uii ON uii.InvoiceId = i.Id ");

            StringBuilder whereBuilder = new StringBuilder();
            string whereCondition = string.Empty;

            if(!string.IsNullOrEmpty(request.CustomerCode))
            {
                whereBuilder.AppendFormat("{0}ISNULL(c.Code, '') LIKE @CustomerCode ",
                                          whereCondition);
                whereCondition = " AND ";
            }

            if(!string.IsNullOrEmpty(request.CustomerName))
            {
                whereBuilder.AppendFormat("{0}ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @CustomerName COLLATE Latin1_General_CI_AI",
                                          whereCondition);
                whereCondition = " AND ";
            }

            if(!string.IsNullOrEmpty(request.CustomerPhone))
            {
                whereBuilder.AppendFormat("{0}(ISNULL(c.Phone, '') LIKE @CustomerPhone OR ISNULL(c.Phone2, '') LIKE @CustomerPhone OR ISNULL(c.Phone3, '') LIKE @CustomerPhone)",
                                          whereCondition);
                whereCondition = " AND ";
            }

            if(!string.IsNullOrEmpty(request.Code))
            {
                whereBuilder.AppendFormat("{0}i.Code LIKE @Code",
                                          whereCondition);
                whereCondition = " AND ";
            }

            if(request.BranchId > 0)
            {
                whereBuilder.AppendFormat("{0}ISNULL(i.BranchId, 0) = @BranchId",
                                          whereCondition);
                whereCondition = " AND ";
            }

            if(request.InvoiceStatusId > 0)
            {
                whereBuilder.AppendFormat("{0}ISNULL(i.InvoiceStatusId, 0) = @InvoiceStatusId",
                                          whereCondition);
                whereCondition = " AND ";
            }

            var isDefaultInvoiceRoles = await _userService.IsBelongDefaultInvoiceRole(userId);
            if(!isDefaultInvoiceRoles.IsSuccess)
            {
                whereBuilder.AppendFormat("{0}(i.UserCreated = @UserCreatedId OR i.UserSellId = @UserSellId OR uii.UserId = @UserDeliveryId)",
                                          whereCondition);
                whereCondition = " AND ";
            }
            else
            {
                if(request.UserCreatedId > 0)
                {
                    whereBuilder.AppendFormat("{0}i.UserCreated = @UserCreatedId",
                                              whereCondition);
                    whereCondition = " AND ";
                }

                if(request.UserSellId > 0)
                {
                    whereBuilder.AppendFormat("{0}i.UserSellId = @UserSellId",
                                              whereCondition);
                    whereCondition = " AND ";
                }

                if(request.UserDeliveryId > 0)
                {
                    whereBuilder.AppendFormat("{0}uii.UserId = @UserDeliveryId AND uii.Type = 0",
                                              whereCondition);
                    whereCondition = " AND ";
                }
            }

            if(request.FromDate != null)
            {
                whereBuilder.AppendFormat("{0}i.DateCreated >= @FromDate",
                                          whereCondition);
                whereCondition = " AND ";
            }

            if(request.ToDate != null)
            {
                whereBuilder.AppendFormat("{0}i.DateCreated <= @ToDate",
                                          whereCondition);
                whereCondition = " AND ";
            }

            whereBuilder.AppendFormat("{0}i.IsDeleted = 0 AND c.IsDeleted = 0 AND uii.IsDeleted = 0",
                                      whereCondition);

            if(!string.IsNullOrEmpty(whereBuilder.ToString()))
            {
                sqlBuilder.AppendFormat( "WHERE {0} ",
                                        whereBuilder);
            }

            sqlBuilder.Append(@"SELECT i.Id,
                                       i.Code,
                                       ins.Name AS InvoiceStatus,
                                       ins.Color AS InvoiceStatusColor,
                                       c.Name AS CustomerName,
                                       c.Phone AS CustomerPhone,
                                       c.Phone2 AS CustomerPhone2,
                                       c.Phone3 AS CustomerPhone3,
                                       c.Address AS CustomerAddress,
                                       p.Type + ' ' + p.Name AS Province,
                                       d.Type + ' ' + d.Name AS District,
                                       w.Type + ' ' + w.Name AS Ward,
                                       c.Code AS CustomerCode,
                                       b.Name AS Branch,
                                       u1.DisplayName AS UserCreated,
                                       u1.PhoneNumber AS UserCreatedPhone,
                                       u2.DisplayName AS UserSell,
                                       u2.PhoneNumber AS UserSellPhone,
                                       i.DeliveryDate,
                                       i.DateCreated
                                FROM #tmpInvoice tmp
                                    INNER JOIN dbo.Invoice i
                                        ON tmp.Id = i.Id
                                    LEFT JOIN dbo.Customer c
                                        ON c.Id = i.CustomerId
                                    LEFT JOIN dbo.Province p
                                        ON p.Id = c.ProvinceId
                                    LEFT JOIN dbo.District d
                                        ON d.Id = c.DistrictId
                                    LEFT JOIN dbo.Ward w
                                        ON w.Id = c.WardId
                                    LEFT JOIN dbo.Branch b
                                        ON b.Id = i.BranchId
                                    LEFT JOIN dbo.InvoiceStatus ins
                                        ON ins.Id = i.InvoiceStatusId
                                    LEFT JOIN dbo.[User] u1
                                        ON u1.Id = i.UserCreated
                                    LEFT JOIN dbo.[User] u2
                                        ON u2.Id = i.UserSellId
                                ORDER BY i.DateCreated DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
              
                                SELECT COUNT(*)
                                FROM #tmpInvoice;
                                DROP TABLE #tmpInvoice;");

            return sqlBuilder.ToString();
        }

        private async Task<string> BuildInvoiceCode(DateTime date)
        {
            try
            {
                var invoicePrefixResponse = await _appConfigService.GetByName("InvoicePrefix");
                string invoicePrefix = "HD";
                if (invoicePrefixResponse.IsSuccess)
                {
                    invoicePrefix = invoicePrefixResponse.Data;
                }

                var code = $"{invoicePrefix}{(date.Year % 100):00}{date.Month:00}{date.Day:00}";
                var countResponses = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.INVOICE_COUNT_BY_DATE,
                                                                                            new
                                                                                            {
                                                                                                    Code = $"{code}%"
                                                                                            });
                if (countResponses.IsSuccess)
                {
                    return $"{code}{(countResponses.Data + 1):000}";
                }

                return string.Empty;
            }
            catch (Exception exception)
            {
                //
            }

            return string.Empty;
        }

        private async Task<int> GetDefaultInvoiceStatus()
        {
            try
            {
                var defaultInvoiceStatus = await _appConfigService.GetByName("DefaultInvoiceStatus");
                int invoiceStatusId = 0;
                if(defaultInvoiceStatus.IsSuccess)
                {
                    if(int.TryParse(defaultInvoiceStatus.Data,out invoiceStatusId))
                    {
                        return invoiceStatusId;
                    }
                }
            }
            catch (Exception exception)
            {
                //
            }

            return 0;
        }

        private async Task<TResponse<bool>> Notify(int userId,
                                                   List<int> userReceiveIds,
                                                   string title,
                                                   string message,
                                                   string code,
                                                   int invoiceId)
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
    }
}

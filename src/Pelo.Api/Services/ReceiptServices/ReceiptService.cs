using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.User;
using Pelo.Common.Dtos.Receipt;
using Pelo.Common.Enums;
using Pelo.Common.Extensions;
using Pelo.Common.Kafka;
using Pelo.Common.Models;
using Pelo.Common.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using Pelo.Common.Dtos;
using Pelo.Common.Events.Receipt;

namespace Pelo.Api.Services.ReceiptServices
{
    public interface IReceiptService
    {
        Task<TResponse<PageResult<GetReceiptPagingResponse>>> GetByCustomerId(int userId,
                                                                              int customerId,
                                                                              int page,
                                                                              int pageSize);
        Task<TResponse<PageResult<GetReceiptPagingResponse>>> GetPaging(int userId,
                                                                        GetReceiptPagingRequest request);

        Task<TResponse<bool>> Insert(int userId,
                                     InsertReceiptRequest request);

        Task<TResponse<GetReceiptByIdResponse>> GetById(int userId,
                                                        int id);
        Task<TResponse<IEnumerable<ReceiptLogResponse>>> GetLogs(int v, int id);
        Task<TResponse<bool>> UpdateCrm(int v, UpdateReceiptRequest request);
        Task<TResponse<bool>> Comment(int v, CommentReceiptRequest commentReceiptRequest);
    }
    public class ReceiptService : BaseService, IReceiptService
    {
        private readonly IAppConfigService _appConfigService;

        private readonly IBusPublisher _busPublisher;

        private readonly IProductService _productService;

        private readonly IRoleService _roleService;

        private readonly IUserService _userService;

        private readonly IConfiguration _configuration;
        public ReceiptService(IDapperReadOnlyRepository readOnlyRepository, IDapperWriteRepository writeRepository, IHttpContextAccessor context, IRoleService roleService,
                              IUserService userService,
                              IProductService productService,
                              IAppConfigService appConfigService, IBusPublisher busPublisher, IConfiguration configuration) : base(readOnlyRepository, writeRepository, context)
        {
            _roleService = roleService;
            _appConfigService = appConfigService;
            _userService = userService;
            _productService = productService;
            _busPublisher = busPublisher;
            _configuration = configuration;
        }
        private async Task<string> BuildSqlQueryGetPaging(int userId,
                                                          GetReceiptPagingRequest request)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("DROP TABLE IF EXISTS #tmpReceipt; ");
            sqlBuilder.Append("SELECT w.Id INTO #tmpReceipt FROM dbo.Receipt w ");
            sqlBuilder.Append("LEFT JOIN dbo.Customer c ON c.Id = w.CustomerId ");
            sqlBuilder.Append("LEFT JOIN dbo.UserInReceipt uiw ON uiw.ReceiptId = w.Id ");

            StringBuilder whereBuilder = new StringBuilder();
            string whereCondition = string.Empty;
            if (!string.IsNullOrEmpty(request.CustomerName))
            {
                whereBuilder.AppendFormat("{0} ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @CustomerName COLLATE Latin1_General_CI_AI",
                                          whereCondition);
                whereCondition = " AND ";
            }

            if (!string.IsNullOrEmpty(request.CustomerPhone))
            {
                whereBuilder.AppendFormat("{0} (ISNULL(c.Phone, '') LIKE @CustomerPhone OR ISNULL(c.Phone2, '') LIKE @CustomerPhone OR ISNULL(c.Phone3, '') LIKE @CustomerPhone)",
                                          whereCondition);
                whereCondition = " AND ";
            }

            if (!string.IsNullOrEmpty(request.Code))
            {
                whereBuilder.AppendFormat("{0} w.Code LIKE @Code",
                                          whereCondition);
                whereCondition = " AND ";
            }

            if (request.ReceiptStatusId > 0)
            {
                whereBuilder.AppendFormat("{0} ISNULL(w.ReceiptStatusId, 0) = @ReceiptStatusId",
                                          whereCondition);
                whereCondition = " AND ";
            }

            var isDefaultReceiptRoles = await _userService.IsBelongDefaultReceiptRole(userId);
            if (!isDefaultReceiptRoles.IsSuccess)
            {
                whereBuilder.AppendFormat("{0} (w.UserCreated = @UserCreatedId)",
                                          whereCondition);
                whereCondition = " AND ";
            }
            else
            {
                if (request.UserCreatedId > 0)
                {
                    whereBuilder.AppendFormat("{0} w.UserCreated = @UserCreatedId",
                                              whereCondition);
                    whereCondition = " AND ";
                }
                if (request.UserCareId > 0)
                {
                    whereBuilder.AppendFormat("{0} uiw.UserId = @UserCareId",
                                              whereCondition);
                    whereCondition = " AND ";
                }
            }
            if (!string.IsNullOrEmpty(request.FromDate))
            {
                whereBuilder.AppendFormat("{0} w.DeliveryDate >= @FromDate",
                                          whereCondition);
                whereCondition = " AND ";
            }

            if (!string.IsNullOrEmpty(request.ToDate))
            {
                whereBuilder.AppendFormat("{0} w.DeliveryDate <= @ToDate",
                                          whereCondition);
                whereCondition = " AND ";
            }

            whereBuilder.AppendFormat("{0} w.IsDeleted = 0 AND c.IsDeleted = 0 AND uiw.IsDeleted = 0",
                                      whereCondition);

            if (!string.IsNullOrEmpty(whereBuilder.ToString()))
            {
                sqlBuilder.AppendFormat("WHERE {0} ",
                                        whereBuilder);
            }

            sqlBuilder.Append(@"SELECT w.Id,
                                       w.Code,
                                       wns.Name AS ReceiptStatus,
                                       wns.Color AS ReceiptStatusColor,
                                       c.Name AS CustomerName,
                                       c.Phone AS CustomerPhone1,
                                       c.Phone2 AS CustomerPhone2,
                                       c.Phone3 AS CustomerPhone3,
                                       c.Address AS CustomerAddress,
                                       c.Code AS CustomerCode,
                                       b.Name AS Branch,
                                       u1.DisplayName AS UserCreated,
                                       u1.PhoneNumber AS UserCreatedPhone,
                                       w.DeliveryDate,
                                       w.DateCreated
                                FROM #tmpReceipt tmp
                                    INNER JOIN dbo.Receipt w
                                        ON tmp.Id = w.Id
                                    LEFT JOIN dbo.Customer c
                                        ON c.Id = w.CustomerId
                                    LEFT JOIN dbo.Province p
                                        ON p.Id = c.ProvinceId
                                    LEFT JOIN dbo.District d
                                        ON d.Id = c.DistrictId
                                    LEFT JOIN dbo.Ward w
                                        ON w.Id = c.WardId
                                    LEFT JOIN dbo.Branch b
                                        ON b.Id = w.BranchId
                                    LEFT JOIN dbo.ReceiptStatus wns
                                        ON ins.Id = w.ReceiptStatusId
                                    LEFT JOIN dbo.[User] u1
                                        ON u1.Id = w.UserCreated
                                ORDER BY w.DateCreated DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                                SELECT COUNT(*)
                                FROM #tmpReceipt;
                                DROP TABLE #tmpReceipt;");

            return sqlBuilder.ToString();
        }
        public async Task<TResponse<PageResult<GetReceiptPagingResponse>>> GetPaging(int userId, GetReceiptPagingRequest request)
        {
            try
            //DefaultReceiptAcceptRoles
            {
                var canGetPaging = await CanGetPaging(userId);
                if (canGetPaging.IsSuccess)
                {
                    var isDefaultReceiptRoles = await _userService.IsBelongDefaultReceiptRole(userId);
                    if (!isDefaultReceiptRoles.IsSuccess)
                    {
                        request.UserCreatedId = userId;
                    }
                    var sqlQuery = await BuildSqlQueryGetPaging(userId,
                                                                request);
                    string fromDate = string.Empty; string toDate = string.Empty;
                    if (!string.IsNullOrEmpty(request.FromDate))
                    {
                        fromDate = string.Format("{0:yyyy-MM-dd} 00:00:00", DateTime.Parse(request.FromDate));
                    }
                    if (!string.IsNullOrEmpty(request.ToDate))
                    {
                        toDate = string.Format("{0:yyyy-MM-dd} 23:59:00", DateTime.Parse(request.ToDate));
                    }
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetReceiptPagingResponse, int>(sqlQuery,
                                                                                                              new
                                                                                                              {
                                                                                                                  CustomerName = $"%{request.CustomerName}%",
                                                                                                                  CustomerPhone = $"%{request.CustomerPhone}%",
                                                                                                                  Code = $"%{request.Code}%",
                                                                                                                  request.ReceiptStatusId,
                                                                                                                  request.UserCreatedId,
                                                                                                                  request.FromDate,
                                                                                                                  request.ToDate,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        foreach (var Receipt in result.Data.Item1)
                        {
                            Receipt.Products = new List<ProductInReceiptSimple>();
                            var products = await ReadOnlyRepository.QueryAsync<ProductInReceiptSimple>(SqlQuery.PRODUCTS_IN_INVOICE_GET_BY_INVOICE_ID,
                                                                                                            new
                                                                                                            {
                                                                                                                ReceiptId = Receipt.Id
                                                                                                            });
                            if (products.IsSuccess
                               && products.Data != null)
                            {
                                Receipt.Products.AddRange(products.Data);
                            }
                        }

                        return await Ok(new PageResult<GetReceiptPagingResponse>(request.Page,
                                                                                 request.PageSize,
                                                                                 result.Data.Item2,
                                                                                 result.Data.Item1));
                    }
                    return await Fail<PageResult<GetReceiptPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetReceiptPagingResponse>>(canGetPaging.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetReceiptPagingResponse>>(exception);
            }
        }
        private async Task<TResponse<bool>> CanGetPaging(int userId)
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

        public async Task<TResponse<PageResult<GetReceiptPagingResponse>>> GetByCustomerId(int userId, int customerId, int page, int pageSize)
        {
            try
            {
                var result = new TResponse<(IEnumerable<GetReceiptPagingResponse>, int)>();

                bool canGetAll = false;

                var canGetAllCrm = await _appConfigService.GetByName("DefaultReceiptAcceptRoles");
                if (canGetAllCrm.IsSuccess)
                {
                    var defaultRoles = canGetAllCrm.Data.Split(" ");
                    var currentRole = await _roleService.GetNameByUserId(userId);
                    if (currentRole.IsSuccess
                       && !string.IsNullOrEmpty(currentRole.Data)
                       && defaultRoles.Contains(currentRole.Data))
                    {
                        canGetAll = true;
                    }
                }

                if (canGetAll)
                {
                    result = await ReadOnlyRepository.QueryMultipleLFAsync<GetReceiptPagingResponse, int>(SqlQuery.RECEIPT_GET_BY_CUSTOMER_ID,
                                                                                                          new
                                                                                                          {
                                                                                                              CustomerId = customerId,
                                                                                                              Skip = (page - 1) * pageSize,
                                                                                                              Take = pageSize
                                                                                                          });
                }
                else
                {
                    result = await ReadOnlyRepository.QueryMultipleLFAsync<GetReceiptPagingResponse, int>(SqlQuery.RECEIPT_GET_BY_CUSTOMER_ID_2,
                                                                                                          new
                                                                                                          {
                                                                                                              CustomerId = customerId,
                                                                                                              UserId = userId,
                                                                                                              Skip = (page - 1) * pageSize,
                                                                                                              Take = pageSize
                                                                                                          });
                }

                if (result.IsSuccess)
                {
                    foreach (var Receipt in result.Data.Item1)
                    {
                        Receipt.Products = new List<ProductInReceiptSimple>();
                        var products = await ReadOnlyRepository.QueryAsync<ProductInReceiptSimple>(SqlQuery.PRODUCTS_IN_RECEIPT_GET_BY_RECEIPT_ID,
                                                                                                        new
                                                                                                        {
                                                                                                            ReceiptId = Receipt.Id
                                                                                                        });
                        if (products.IsSuccess
                           && products.Data != null)
                        {
                            Receipt.Products.AddRange(products.Data);
                        }
                    }

                    return await Ok(new PageResult<GetReceiptPagingResponse>(page,
                                                                             pageSize,
                                                                             result.Data.Item2,
                                                                             result.Data.Item1));
                }

                return await Fail<PageResult<GetReceiptPagingResponse>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetReceiptPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertReceiptRequest request)
        {
            try
            {
                var canInsert = await CanInsert(userId);
                if (canInsert.IsSuccess)
                {
                    var code = await BuildInvoiceCode(DateTime.Now);
                    var ReceiptStatusId = await GetDefaultReceiptStatus();
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.RECEIPT_INSERT,
                                                                               new
                                                                               {
                                                                                   Code = code,
                                                                                   ReceiptStatusId = ReceiptStatusId,
                                                                                   request.BranchId,
                                                                                   request.CustomerId,
                                                                                   request.Total,
                                                                                   request.Deposit,
                                                                                   request.DeliveryDate,
                                                                                   request.Description,
                                                                                   UserCreated = userId,
                                                                                   UserUpdated = userId
                                                                               });

                    if (result.IsSuccess)
                    {
                        var ReceiptId = result.Data;

                        #region 1. Thêm sản phẩm

                        if (request.Products != null)
                        {
                            foreach (var productIReceiptRequest in request.Products)
                            {
                                var product = await _productService.GetSimpleById(productIReceiptRequest.Id);
                                if (product != null)
                                {
                                    var resultAddProduct = await WriteRepository.ExecuteAsync(SqlQuery.PRODUCT_IN_RECEIPT_INSERT,
                                                                                              new
                                                                                              {
                                                                                                  ProductId = productIReceiptRequest.Id,
                                                                                                  ReceiptId = ReceiptId,
                                                                                                  ProductName = product.Name,
                                                                                                  productIReceiptRequest.Description,
                                                                                                  productIReceiptRequest.ReceiptDescriptionId,
                                                                                                  productIReceiptRequest.SerialNumber,
                                                                                                  UserCreated = userId,
                                                                                                  UserUpdated = userId
                                                                                              });
                                }
                            }
                        }

                        #endregion

                        #region 2. Thêm thông báo

                        await _busPublisher.SendEventAsync(new ReceiptInsertSuccessEvent
                        {
                            Id = ReceiptId,
                            Code = code,
                            ReceiptStatusId = ReceiptStatusId,
                            UserId = userId,
                            CustomerId = request.CustomerId
                        });

                        #endregion

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

        public async Task<TResponse<GetReceiptByIdResponse>> GetById(int userId, int id)
        {
            try
            {
                var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetReceiptByIdResponse>(SqlQuery.RECEIPT_GET_BY_ID, new
                {
                    Id = id
                });
                if (result.IsSuccess)
                {
                    if (result.Data != null)
                    {
                        var Receipt = result.Data;

                        Receipt.Products = new List<ProductInReceipt>();
                        Receipt.UsersCare = new List<UserDisplaySimpleModel>();
                        Receipt.UsersInCharge = new List<UserDisplaySimpleModel>();

                        var products = await ReadOnlyRepository.QueryAsync<ProductInReceipt>(SqlQuery.GET_PRODUCTS_IN_RECEIPT, new
                        {
                            ReceiptId = id
                        });
                        if (products.IsSuccess
                           && products.Data != null)
                        {
                            Receipt.Products.AddRange(products.Data);
                        }

                        var userCares = await ReadOnlyRepository.QueryAsync<UserDisplaySimpleModel>(SqlQuery.GET_USERS_IN_RECEIPT, new
                        {
                            ReceiptId = id,
                            Type = 1
                        });
                        if (userCares.IsSuccess
                           && userCares.Data != null)
                        {
                            Receipt.UsersCare.AddRange(userCares.Data);
                        }

                        var userIncharge = await ReadOnlyRepository.QueryAsync<UserDisplaySimpleModel>(SqlQuery.GET_USERS_IN_RECEIPT, new
                        {
                            ReceiptId = id,
                            Type = 0
                        });
                        if (userIncharge.IsSuccess
                           && userIncharge.Data != null)
                        {
                            Receipt.UsersInCharge.AddRange(userIncharge.Data);
                        }

                        return await Ok(Receipt);
                    }

                    return await Fail<GetReceiptByIdResponse>("Not found");
                }

                return await Fail<GetReceiptByIdResponse>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetReceiptByIdResponse>(exception);
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

        private async Task<string> BuildInvoiceCode(DateTime date)
        {
            try
            {
                var ReceiptPrefixResponse = await _appConfigService.GetByName("ReceiptPrefix");
                string ReceiptPrefix = "BH";
                if (ReceiptPrefixResponse.IsSuccess)
                {
                    ReceiptPrefix = ReceiptPrefixResponse.Data;
                }

                if (string.IsNullOrEmpty(ReceiptPrefix))
                {
                    ReceiptPrefix = "BH";
                }

                var code = $"{ReceiptPrefix}{(date.Year % 100):00}{date.Month:00}{date.Day:00}";
                var countResponses = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.RECEIPT_COUNT_BY_DATE,
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

        private async Task<int> GetDefaultReceiptStatus()
        {
            try
            {
                var defaultReceiptStatus = await _appConfigService.GetByName("DefaultReceiptStatus");
                int ReceiptStatusId = 0;
                if (defaultReceiptStatus.IsSuccess)
                {
                    if (int.TryParse(defaultReceiptStatus.Data, out ReceiptStatusId))
                    {
                        return ReceiptStatusId;
                    }
                }
            }
            catch (Exception exception)
            {
                //
            }

            return 0;
        }

        public async Task<TResponse<IEnumerable<ReceiptLogResponse>>> GetLogs(int userId, int id)
        {
            try
            {
                var result = await ReadOnlyRepository.QueryAsync<ReceiptLogResponse>(SqlQuery.RECEIPT_GET_LOGS,
                                                                                 new
                                                                                 {
                                                                                     ReceiptId = id
                                                                                 });
                if (result.IsSuccess)
                {
                    foreach (var log in result.Data)
                    {
                        var user = await ReadOnlyRepository.QueryFirstOrDefaultAsync<UserInLog>(SqlQuery.GET_USER_IN_LOG,
                                                                                                new
                                                                                                {
                                                                                                    Id = log.UserId
                                                                                                });
                        if (user.IsSuccess
                           && user.Data != null)
                        {
                            log.User = user.Data;
                        }

                        var oldCrmStatus = await ReadOnlyRepository.QueryFirstOrDefaultAsync<StatusInLog>(SqlQuery.GET_RECEIPT_STATUS_IN_LOG,
                                                                                                             new
                                                                                                             {
                                                                                                                 Id = log.OldReceiptStatusId
                                                                                                             });
                        if (oldCrmStatus.IsSuccess)
                        {
                            log.OldReceiptStatus = oldCrmStatus.Data;
                        }

                        var crmStatus = await ReadOnlyRepository.QueryFirstOrDefaultAsync<StatusInLog>(SqlQuery.GET_RECEIPT_STATUS_IN_LOG,
                                                                                                          new
                                                                                                          {
                                                                                                              Id = log.ReceiptStatusId
                                                                                                          });
                        if (crmStatus.IsSuccess)
                        {
                            log.ReceiptStatus = crmStatus.Data;
                        }

                        var attachments = await ReadOnlyRepository.QueryAsync<LogAttachment>(SqlQuery.GET_RECEIPT_ATTACHMENT_IN_LOG,
                                                                                                new
                                                                                                {
                                                                                                    ReceiptLogId = log.Id
                                                                                                });

                        if (attachments.IsSuccess)
                        {
                            log.Attachments = attachments.Data.ToList();
                        }
                    }

                    return await Ok(result.Data);
                }

                return await Fail<IEnumerable<ReceiptLogResponse>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<ReceiptLogResponse>>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                                             "GetReceiptLog"));
            }
        }

        public async Task<TResponse<bool>> UpdateCrm(int userId, UpdateReceiptRequest request)
        {
            try
            {
                var oldInformation = await CanUpdate(userId,
                                                     request);
                if (oldInformation.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.RECEIPT_UPDATE,
                                                                    new
                                                                    {
                                                                        request.Id,
                                                                        request.DeliveryDate,
                                                                        request.Total,
                                                                        request.Deposit,
                                                                        request.BranchId,
                                                                        request.Description,
                                                                        UserUpdated = userId,
                                                                        DateUpdated = DateTime.Now
                                                                    });
                    if (result.IsSuccess)
                    {
                        if (result.Data > 0)
                        {
                            var ReceiptId = request.Id;
                            var ReceiptCareUser = await ReadOnlyRepository.QueryAsync<ReceiptUserResponse>(SqlQuery.GET_RECEIPT_USER_BY_RECEIPTID,
                                                                                               new
                                                                                               {
                                                                                                   ReceiptId = ReceiptId,
                                                                                                   Type = 0
                                                                                               });
                            var ReceiptRelativeUser = await ReadOnlyRepository.QueryAsync<ReceiptUserResponse>(SqlQuery.GET_RECEIPT_USER_BY_RECEIPTID,
                                                                                               new
                                                                                               {
                                                                                                   ReceiptId = ReceiptId,
                                                                                                   Type = 1
                                                                                               });
                            if (request.UserCareIds == null)
                            {
                                request.UserCareIds = new List<int>();
                            }

                            if (ReceiptCareUser.Data.Any()
                               || request.UserCareIds.Any())
                            {
                                if (ReceiptCareUser.Data.Any()
                                   && (request.UserCareIds == null || !request.UserCareIds.Any()))
                                {
                                }
                                else if (request.UserCareIds.Any()
                                        && (ReceiptCareUser.Data == null || !ReceiptCareUser.Data.Any()))
                                {
                                }
                            }
                            if (request.UserRelativeIds == null)
                            {
                                request.UserRelativeIds = new List<int>();
                            }

                            if (ReceiptRelativeUser.Data.Any()
                               || request.UserRelativeIds.Any())
                            {
                                if (ReceiptRelativeUser.Data.Any()
                                   && (request.UserRelativeIds == null || !request.UserRelativeIds.Any()))
                                {
                                }
                                else if (request.UserRelativeIds.Any()
                                        && (ReceiptRelativeUser.Data == null || !ReceiptRelativeUser.Data.Any()))
                                {
                                }
                            }

                            List<int> userCareIds = new List<int>();
                            List<int> userRelativeIds = new List<int>();
                            if (ReceiptCareUser != null)
                            {
                                if (request.UserCareIds.Any())
                                {
                                    ReceiptUserResponse[] ReceiptUserResponse = ReceiptCareUser.Data.Where(c => c.ReceiptId == request.Id && !request.UserCareIds.Contains(c.UserId))
                                                                               .ToArray();
                                    userCareIds.AddRange(ReceiptUserResponse.Select(c => c.UserId));
                                    foreach (var item in ReceiptUserResponse)
                                    {
                                        await WriteRepository.ExecuteAsync(SqlQuery.RECEIPT_USER_DELETE,
                                                                           new
                                                                           {
                                                                               ReceiptId = request.Id,
                                                                               UserId = item.UserId,
                                                                               Type = 0
                                                                           });
                                    }

                                    foreach (var user in request.UserCareIds)
                                    {
                                        var rs = await WriteRepository.ExecuteAsync(SqlQuery.RECEIPT_USER_INSERT,
                                                                                    new
                                                                                    {
                                                                                        ReceiptId = request.Id,
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
                                    if (ReceiptCareUser.IsSuccess)
                                    {
                                        foreach (var item in ReceiptCareUser.Data)
                                        {
                                            var rs = await WriteRepository.ExecuteAsync(SqlQuery.RECEIPT_USER_DELETE,
                                                                                        new
                                                                                        {
                                                                                            ReceiptId = request.Id,
                                                                                            UserId = item.Id,
                                                                                            Type = 0
                                                                                        });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var user in request.UserCareIds)
                                {
                                    var rs = await WriteRepository.ExecuteAsync(SqlQuery.RECEIPT_USER_INSERT,
                                                                                new
                                                                                {
                                                                                    ReceiptId = request.Id,
                                                                                    UserId = user,
                                                                                    Type = 0,
                                                                                    UserUpdated = userId,
                                                                                    UserCreated = userId,
                                                                                    DateUpdated = DateTime.Now,
                                                                                    DateCreated = DateTime.Now
                                                                                });
                                }
                            }
                            if (ReceiptRelativeUser != null)
                            {
                                if (request.UserRelativeIds.Any())
                                {
                                    ReceiptUserResponse[] ReceiptUserResponse = ReceiptRelativeUser.Data.Where(c => c.ReceiptId == request.Id && !request.UserRelativeIds.Contains(c.UserId))
                                                                               .ToArray();
                                    userRelativeIds.AddRange(ReceiptUserResponse.Select(c => c.UserId));
                                    foreach (var item in ReceiptUserResponse)
                                    {
                                        await WriteRepository.ExecuteAsync(SqlQuery.RECEIPT_USER_DELETE,
                                                                           new
                                                                           {
                                                                               ReceiptId = request.Id,
                                                                               UserId = item.UserId,
                                                                               Type = 1
                                                                           });
                                    }

                                    foreach (var user in request.UserRelativeIds)
                                    {
                                        var rs = await WriteRepository.ExecuteAsync(SqlQuery.RECEIPT_USER_INSERT,
                                                                                    new
                                                                                    {
                                                                                        ReceiptId = request.Id,
                                                                                        UserId = user,
                                                                                        Type = 1,
                                                                                        UserUpdated = userId,
                                                                                        UserCreated = userId,
                                                                                        DateUpdated = DateTime.Now,
                                                                                        DateCreated = DateTime.Now
                                                                                    });
                                    }
                                }
                                else
                                {
                                    if (ReceiptRelativeUser.IsSuccess)
                                    {
                                        foreach (var item in ReceiptRelativeUser.Data)
                                        {
                                            var rs = await WriteRepository.ExecuteAsync(SqlQuery.RECEIPT_USER_DELETE,
                                                                                        new
                                                                                        {
                                                                                            ReceiptId = request.Id,
                                                                                            UserId = item.Id,
                                                                                            Type = 1
                                                                                        });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var user in request.UserCareIds)
                                {
                                    var rs = await WriteRepository.ExecuteAsync(SqlQuery.RECEIPT_USER_INSERT,
                                                                                new
                                                                                {
                                                                                    ReceiptId = request.Id,
                                                                                    UserId = user,
                                                                                    Type = 0,
                                                                                    UserUpdated = userId,
                                                                                    UserCreated = userId,
                                                                                    DateUpdated = DateTime.Now,
                                                                                    DateCreated = DateTime.Now
                                                                                });
                                }
                            }
                            await _busPublisher.SendEventAsync(new ReceiptUpdateSuccessEvent
                            {
                                Id = oldInformation.Data.Id,
                                Code = oldInformation.Data.Code,
                                BeforeUpdated = new ReceiptUpdateSuccessModel
                                {
                                    ReceiptStatusId = oldInformation.Data.ReceiptStatusId,
                                    DeliveryDate = oldInformation.Data.DeliveryDate,
                                    Description = oldInformation.Data.Description,
                                    UserCareIds = userCareIds,
                                    UserRelativeIds = userRelativeIds
                                },
                                AfterUpdated = new ReceiptUpdateSuccessModel
                                {
                                    ReceiptStatusId = oldInformation.Data.ReceiptStatusId,
                                    DeliveryDate = request.DeliveryDate,
                                    Description = request.Description,
                                    UserRelativeIds = request.UserRelativeIds,
                                    UserCareIds = request.UserRelativeIds
                                },
                                UserId = userId
                            });

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

        private async Task<TResponse<GetReceiptByIdResponse>> CanUpdate(int userId,
                                                                    UpdateReceiptRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if (checkPermission.IsSuccess)
                {
                    if (request.Id == 0)
                    {
                        return await Fail<GetReceiptByIdResponse>(ErrorEnum.CRM_HAS_NOT_EXIST.GetStringValue());
                    }

                    var checkIdInvalid = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetReceiptByIdResponse>(SqlQuery.RECEIPT_GET_BY_ID,
                                                                                                               new
                                                                                                               {
                                                                                                                   request.Id
                                                                                                               });
                    if (checkIdInvalid.IsSuccess)
                    {
                        if (checkIdInvalid.Data != null)
                        {
                            return await Ok(checkIdInvalid.Data);
                        }

                        return await Fail<GetReceiptByIdResponse>(ErrorEnum.CRM_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<GetReceiptByIdResponse>(checkIdInvalid.Message);
                }

                return await Fail<GetReceiptByIdResponse>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetReceiptByIdResponse>(exception);
            }
        }
        //TODO
        public async Task<TResponse<bool>> Comment(int userId, CommentReceiptRequest request)
        {
            try
            {
                var Receipt = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetReceiptByIdResponse>(SqlQuery.RECEIPT_GET_BY_ID,
                                                                                                new
                                                                                                {
                                                                                                    request.Id
                                                                                                });
                if (Receipt.IsSuccess)
                {
                    if (Receipt.Data != null)
                    {
                        var rs1 = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.STATUS_RECEIPT_UPDATE, new { request.Id, request.ReceiptStatusId });
                        if (rs1.IsSuccess)
                        {
                            var rs = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.RECEIPT_INSERT_COMMENT,
                                                                               new
                                                                               {
                                                                                   ReceiptId = request.Id,
                                                                                   Comment = string.IsNullOrEmpty(request.Comment)
                                                                                                         ? "đã đính kèm file"
                                                                                                         : request.Comment,
                                                                                   UserId = userId,
                                                                                   Receipt.Data.ReceiptStatusId,
                                                                                   OldReceiptStatusId = Receipt.Data.ReceiptStatusId
                                                                               });
                            if (rs.IsSuccess)
                            {
                                if (request.Files != null
                                   && request.Files.Any())
                                {
                                    var crmLogId = rs.Data;

                                    var path = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey) + "\\wwwroot\\Attachments";

                                    if (!Directory.Exists(path))
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

                                            var result = await WriteRepository.ExecuteAsync(SqlQuery.RECEIPT_LOG_ATTACHMENT_INSERT,
                                                                                            new
                                                                                            {
                                                                                                ReceiptId = crmLogId,
                                                                                                Attachment = $"{newFileName}{Path.GetExtension(file.FileName)}",
                                                                                                AttachmentName = file.FileName,
                                                                                                UserCreated = userId,
                                                                                                UserUpdated = userId
                                                                                            });
                                        }
                                    }

                                    await _busPublisher.SendEventAsync(new ReceiptCommentSuccessEvent
                                    {
                                        Id = request.Id,
                                        Code = Receipt.Data.Code,
                                        Comment = request.Comment,
                                        HasAttachmentFile = request.Files != null && request.Files.Any(),
                                        UserId = userId,
                                        ReceiptStatusId = request.ReceiptStatusId
                                    });

                                    return await Ok(true);
                                }

                                return await Ok(true);
                            }
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

        private string RenameFile(string fileName)
        {
            var newName = Guid.NewGuid()
                              .ToString()
                              .Replace("-",
                                       string.Empty);
            return newName;
        }
    }
}

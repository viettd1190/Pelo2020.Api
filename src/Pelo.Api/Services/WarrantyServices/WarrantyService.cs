﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.User;
using Pelo.Common.Dtos.Warranty;
using Pelo.Common.Enums;
using Pelo.Common.Extensions;
using Pelo.Common.Events.Warranty;
using Pelo.Common.Kafka;
using Pelo.Common.Models;
using Pelo.Common.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using Pelo.Common.Dtos;

namespace Pelo.Api.Services.WarrantyServices
{
    public interface IWarrantyService
    {
        Task<TResponse<PageResult<GetWarrantyPagingResponse>>> GetByCustomerId(int userId,
                                                                              int customerId,
                                                                              int page,
                                                                              int pageSize);
        Task<TResponse<PageResult<GetWarrantyPagingResponse>>> GetPaging(int userId,
                                                                        GetWarrantyPagingRequest request);

        Task<TResponse<bool>> Insert(int userId,
                                     InsertWarrantyRequest request);

        Task<TResponse<GetWarrantyByIdResponse>> GetById(int userId,
                                                        int id);
        Task<TResponse<IEnumerable<WarrantyLogResponse>>> GetLogs(int v, int id);
        Task<TResponse<bool>> UpdateCrm(int v, UpdateWarrantyRequest request);
        Task<TResponse<bool>> Comment(int v, CommentWarrantyRequest commentWarrantyRequest);
    }
    public class WarrantyService : BaseService, IWarrantyService
    {
        private readonly IAppConfigService _appConfigService;

        private readonly IBusPublisher _busPublisher;

        private readonly IProductService _productService;

        private readonly IRoleService _roleService;

        private readonly IUserService _userService;

        private readonly IConfiguration _configuration;
        public WarrantyService(IDapperReadOnlyRepository readOnlyRepository, IDapperWriteRepository writeRepository, IHttpContextAccessor context, IRoleService roleService,
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
                                                          GetWarrantyPagingRequest request)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("DROP TABLE IF EXISTS #tmpWarranty; ");
            sqlBuilder.Append("SELECT w.Id INTO #tmpWarranty FROM dbo.Warranty w ");
            sqlBuilder.Append("LEFT JOIN dbo.Customer c ON c.Id = w.CustomerId ");
            sqlBuilder.Append("LEFT JOIN dbo.UserInWarranty uiw ON uiw.WarrantyId = w.Id ");

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

            if (request.WarrantyStatusId > 0)
            {
                whereBuilder.AppendFormat("{0} ISNULL(w.WarrantyStatusId, 0) = @WarrantyStatusId",
                                          whereCondition);
                whereCondition = " AND ";
            }

            var isDefaultWarrantyRoles = await _userService.IsBelongDefaultWarrantyRole(userId);
            if (!isDefaultWarrantyRoles.IsSuccess)
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
                                       wns.Name AS WarrantyStatus,
                                       wns.Color AS WarrantyStatusColor,
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
                                FROM #tmpWarranty tmp
                                    INNER JOIN dbo.Warranty w
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
                                    LEFT JOIN dbo.WarrantyStatus wns
                                        ON ins.Id = w.WarrantyStatusId
                                    LEFT JOIN dbo.[User] u1
                                        ON u1.Id = w.UserCreated
                                ORDER BY w.DateCreated DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                                SELECT COUNT(*)
                                FROM #tmpWarranty;
                                DROP TABLE #tmpWarranty;");

            return sqlBuilder.ToString();
        }
        public async Task<TResponse<PageResult<GetWarrantyPagingResponse>>> GetPaging(int userId, GetWarrantyPagingRequest request)
        {
            try
            //DefaultWarrantyAcceptRoles
            {
                var canGetPaging = await CanGetPaging(userId);
                if (canGetPaging.IsSuccess)
                {
                    var isDefaultWarrantyRoles = await _userService.IsBelongDefaultWarrantyRole(userId);
                    if (!isDefaultWarrantyRoles.IsSuccess)
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
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetWarrantyPagingResponse, int>(sqlQuery,
                                                                                                              new
                                                                                                              {
                                                                                                                  CustomerName = $"%{request.CustomerName}%",
                                                                                                                  CustomerPhone = $"%{request.CustomerPhone}%",
                                                                                                                  Code = $"%{request.Code}%",
                                                                                                                  request.WarrantyStatusId,
                                                                                                                  request.UserCreatedId,
                                                                                                                  request.FromDate,
                                                                                                                  request.ToDate,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        foreach (var warranty in result.Data.Item1)
                        {
                            warranty.Products = new List<ProductInWarrantySimple>();
                            var products = await ReadOnlyRepository.QueryAsync<ProductInWarrantySimple>(SqlQuery.PRODUCTS_IN_INVOICE_GET_BY_INVOICE_ID,
                                                                                                            new
                                                                                                            {
                                                                                                                WarrantyId = warranty.Id
                                                                                                            });
                            if (products.IsSuccess
                               && products.Data != null)
                            {
                                warranty.Products.AddRange(products.Data);
                            }
                        }

                        return await Ok(new PageResult<GetWarrantyPagingResponse>(request.Page,
                                                                                 request.PageSize,
                                                                                 result.Data.Item2,
                                                                                 result.Data.Item1));
                    }
                    return await Fail<PageResult<GetWarrantyPagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetWarrantyPagingResponse>>(canGetPaging.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetWarrantyPagingResponse>>(exception);
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

        public async Task<TResponse<PageResult<GetWarrantyPagingResponse>>> GetByCustomerId(int userId, int customerId, int page, int pageSize)
        {
            try
            {
                var result = new TResponse<(IEnumerable<GetWarrantyPagingResponse>, int)>();

                bool canGetAll = false;

                var canGetAllCrm = await _appConfigService.GetByName("DefaultWarrantyAcceptRoles");
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
                    result = await ReadOnlyRepository.QueryMultipleLFAsync<GetWarrantyPagingResponse, int>(SqlQuery.WARRANTY_GET_BY_CUSTOMER_ID,
                                                                                                          new
                                                                                                          {
                                                                                                              CustomerId = customerId,
                                                                                                              Skip = (page - 1) * pageSize,
                                                                                                              Take = pageSize
                                                                                                          });
                }
                else
                {
                    result = await ReadOnlyRepository.QueryMultipleLFAsync<GetWarrantyPagingResponse, int>(SqlQuery.WARRANTY_GET_BY_CUSTOMER_ID_2,
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
                    foreach (var warranty in result.Data.Item1)
                    {
                        warranty.Products = new List<ProductInWarrantySimple>();
                        var products = await ReadOnlyRepository.QueryAsync<ProductInWarrantySimple>(SqlQuery.PRODUCTS_IN_WARRANTY_GET_BY_WARRANTY_ID,
                                                                                                        new
                                                                                                        {
                                                                                                            WarrantyId = warranty.Id
                                                                                                        });
                        if (products.IsSuccess
                           && products.Data != null)
                        {
                            warranty.Products.AddRange(products.Data);
                        }
                    }

                    return await Ok(new PageResult<GetWarrantyPagingResponse>(page,
                                                                             pageSize,
                                                                             result.Data.Item2,
                                                                             result.Data.Item1));
                }

                return await Fail<PageResult<GetWarrantyPagingResponse>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetWarrantyPagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertWarrantyRequest request)
        {
            try
            {
                var canInsert = await CanInsert(userId);
                if (canInsert.IsSuccess)
                {
                    var code = await BuildInvoiceCode(DateTime.Now);
                    var warrantyStatusId = await GetDefaultWarrantyStatus();
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.WARRANTY_INSERT,
                                                                               new
                                                                               {
                                                                                   Code = code,
                                                                                   WarrantyStatusId = warrantyStatusId,
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
                        var warrantyId = result.Data;

                        #region 1. Thêm sản phẩm

                        if (request.Products != null)
                        {
                            foreach (var productIWarrantyRequest in request.Products)
                            {
                                var product = await _productService.GetSimpleById(productIWarrantyRequest.Id);
                                if (product != null)
                                {
                                    var resultAddProduct = await WriteRepository.ExecuteAsync(SqlQuery.PRODUCT_IN_WARRANTY_INSERT,
                                                                                              new
                                                                                              {
                                                                                                  ProductId = productIWarrantyRequest.Id,
                                                                                                  WarrantyId = warrantyId,
                                                                                                  ProductName = product.Name,
                                                                                                  productIWarrantyRequest.Description,
                                                                                                  productIWarrantyRequest.WarrantyDescriptionId,
                                                                                                  productIWarrantyRequest.SerialNumber,
                                                                                                  UserCreated = userId,
                                                                                                  UserUpdated = userId
                                                                                              });
                                }
                            }
                        }

                        #endregion

                        #region 2. Thêm thông báo

                        await _busPublisher.SendEventAsync(new WarrantyInsertSuccessEvent
                        {
                            Id = warrantyId,
                            Code = code,
                            WarrantyStatusId = warrantyStatusId,
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

        public async Task<TResponse<GetWarrantyByIdResponse>> GetById(int userId, int id)
        {
            try
            {
                var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetWarrantyByIdResponse>(SqlQuery.WARRANTY_GET_BY_ID, new
                {
                    Id = id
                });
                if (result.IsSuccess)
                {
                    if (result.Data != null)
                    {
                        var warranty = result.Data;

                        warranty.Products = new List<ProductInWarranty>();
                        warranty.UsersCare = new List<UserDisplaySimpleModel>();
                        warranty.UsersInCharge = new List<UserDisplaySimpleModel>();

                        var products = await ReadOnlyRepository.QueryAsync<ProductInWarranty>(SqlQuery.GET_PRODUCTS_IN_WARRANTY, new
                        {
                            WarrantyId = id
                        });
                        if (products.IsSuccess
                           && products.Data != null)
                        {
                            warranty.Products.AddRange(products.Data);
                        }

                        var userCares = await ReadOnlyRepository.QueryAsync<UserDisplaySimpleModel>(SqlQuery.GET_USERS_IN_WARRANTY, new
                        {
                            WarrantyId = id,
                            Type = 1
                        });
                        if (userCares.IsSuccess
                           && userCares.Data != null)
                        {
                            warranty.UsersCare.AddRange(userCares.Data);
                        }

                        var userIncharge = await ReadOnlyRepository.QueryAsync<UserDisplaySimpleModel>(SqlQuery.GET_USERS_IN_WARRANTY, new
                        {
                            WarrantyId = id,
                            Type = 0
                        });
                        if (userIncharge.IsSuccess
                           && userIncharge.Data != null)
                        {
                            warranty.UsersInCharge.AddRange(userIncharge.Data);
                        }

                        return await Ok(warranty);
                    }

                    return await Fail<GetWarrantyByIdResponse>("Not found");
                }

                return await Fail<GetWarrantyByIdResponse>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetWarrantyByIdResponse>(exception);
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
                var warrantyPrefixResponse = await _appConfigService.GetByName("WarrantyPrefix");
                string warrantyPrefix = "BH";
                if (warrantyPrefixResponse.IsSuccess)
                {
                    warrantyPrefix = warrantyPrefixResponse.Data;
                }

                if (string.IsNullOrEmpty(warrantyPrefix))
                {
                    warrantyPrefix = "BH";
                }

                var code = $"{warrantyPrefix}{(date.Year % 100):00}{date.Month:00}{date.Day:00}";
                var countResponses = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.WARRANTY_COUNT_BY_DATE,
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

        private async Task<int> GetDefaultWarrantyStatus()
        {
            try
            {
                var defaultWarrantyStatus = await _appConfigService.GetByName("DefaultWarrantyStatus");
                int warrantyStatusId = 0;
                if (defaultWarrantyStatus.IsSuccess)
                {
                    if (int.TryParse(defaultWarrantyStatus.Data, out warrantyStatusId))
                    {
                        return warrantyStatusId;
                    }
                }
            }
            catch (Exception exception)
            {
                //
            }

            return 0;
        }

        public async Task<TResponse<IEnumerable<WarrantyLogResponse>>> GetLogs(int userId, int id)
        {
            try
            {
                var result = await ReadOnlyRepository.QueryAsync<WarrantyLogResponse>(SqlQuery.WARRANTY_GET_LOGS,
                                                                                 new
                                                                                 {
                                                                                     WarrantyId = id
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

                        var oldCrmStatus = await ReadOnlyRepository.QueryFirstOrDefaultAsync<StatusInLog>(SqlQuery.GET_WARRANTY_STATUS_IN_LOG,
                                                                                                             new
                                                                                                             {
                                                                                                                 Id = log.OldCrmStatusId
                                                                                                             });
                        if (oldCrmStatus.IsSuccess)
                        {
                            log.OldWarrantyStatus = oldCrmStatus.Data;
                        }

                        var crmStatus = await ReadOnlyRepository.QueryFirstOrDefaultAsync<StatusInLog>(SqlQuery.GET_WARRANTY_STATUS_IN_LOG,
                                                                                                          new
                                                                                                          {
                                                                                                              Id = log.CrmStatusId
                                                                                                          });
                        if (crmStatus.IsSuccess)
                        {
                            log.WarrantyStatus = crmStatus.Data;
                        }

                        var attachments = await ReadOnlyRepository.QueryAsync<LogAttachment>(SqlQuery.GET_WARRANTY_ATTACHMENT_IN_LOG,
                                                                                                new
                                                                                                {
                                                                                                    WarrantyLogId = log.Id
                                                                                                });

                        if (attachments.IsSuccess)
                        {
                            log.Attachments = attachments.Data.ToList();
                        }
                    }

                    return await Ok(result.Data);
                }

                return await Fail<IEnumerable<WarrantyLogResponse>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<WarrantyLogResponse>>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                                             "GetWarrantyLog"));
            }
        }

        public async Task<TResponse<bool>> UpdateCrm(int userId, UpdateWarrantyRequest request)
        {
            try
            {
                var oldInformation = await CanUpdate(userId,
                                                     request);
                if (oldInformation.IsSuccess)
                {
                    var result = await WriteRepository.ExecuteAsync(SqlQuery.WARRANTY_UPDATE,
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
                            var warrantyId = request.Id;
                            var warrantyCareUser = await ReadOnlyRepository.QueryAsync<WarrantyUserResponse>(SqlQuery.GET_WARRANTY_USER_BY_WARRANTYID,
                                                                                               new
                                                                                               {
                                                                                                   WarrantyId = warrantyId,
                                                                                                   Type = 0
                                                                                               });
                            var warrantyRelativeUser = await ReadOnlyRepository.QueryAsync<WarrantyUserResponse>(SqlQuery.GET_WARRANTY_USER_BY_WARRANTYID,
                                                                                               new
                                                                                               {
                                                                                                   WarrantyId = warrantyId,
                                                                                                   Type = 1
                                                                                               });
                            if (request.UserCareIds == null)
                            {
                                request.UserCareIds = new List<int>();
                            }

                            if (warrantyCareUser.Data.Any()
                               || request.UserCareIds.Any())
                            {
                                if (warrantyCareUser.Data.Any()
                                   && (request.UserCareIds == null || !request.UserCareIds.Any()))
                                {
                                }
                                else if (request.UserCareIds.Any()
                                        && (warrantyCareUser.Data == null || !warrantyCareUser.Data.Any()))
                                {
                                }
                            }
                            if (request.UserRelativeIds == null)
                            {
                                request.UserRelativeIds = new List<int>();
                            }

                            if (warrantyRelativeUser.Data.Any()
                               || request.UserRelativeIds.Any())
                            {
                                if (warrantyRelativeUser.Data.Any()
                                   && (request.UserRelativeIds == null || !request.UserRelativeIds.Any()))
                                {
                                }
                                else if (request.UserRelativeIds.Any()
                                        && (warrantyRelativeUser.Data == null || !warrantyRelativeUser.Data.Any()))
                                {
                                }
                            }

                            List<int> userCareIds = new List<int>();
                            List<int> userRelativeIds = new List<int>();
                            if (warrantyCareUser != null)
                            {
                                if (request.UserCareIds.Any())
                                {
                                    WarrantyUserResponse[] warrantyUserResponse = warrantyCareUser.Data.Where(c => c.WarrantyId == request.Id && !request.UserCareIds.Contains(c.UserId))
                                                                               .ToArray();
                                    userCareIds.AddRange(warrantyUserResponse.Select(c => c.UserId));
                                    foreach (var item in warrantyUserResponse)
                                    {
                                        await WriteRepository.ExecuteAsync(SqlQuery.WARRANTY_USER_DELETE,
                                                                           new
                                                                           {
                                                                               WarrantyId = request.Id,
                                                                               UserId = item.UserId,
                                                                               Type = 0
                                                                           });
                                    }

                                    foreach (var user in request.UserCareIds)
                                    {
                                        var rs = await WriteRepository.ExecuteAsync(SqlQuery.WARRANTY_USER_INSERT,
                                                                                    new
                                                                                    {
                                                                                        WarrantyId = request.Id,
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
                                    if (warrantyCareUser.IsSuccess)
                                    {
                                        foreach (var item in warrantyCareUser.Data)
                                        {
                                            var rs = await WriteRepository.ExecuteAsync(SqlQuery.WARRANTY_USER_DELETE,
                                                                                        new
                                                                                        {
                                                                                            WarrantyId = request.Id,
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
                                    var rs = await WriteRepository.ExecuteAsync(SqlQuery.WARRANTY_USER_INSERT,
                                                                                new
                                                                                {
                                                                                    WarrantyId = request.Id,
                                                                                    UserId = user,
                                                                                    Type = 0,
                                                                                    UserUpdated = userId,
                                                                                    UserCreated = userId,
                                                                                    DateUpdated = DateTime.Now,
                                                                                    DateCreated = DateTime.Now
                                                                                });
                                }
                            }
                            if (warrantyRelativeUser != null)
                            {
                                if (request.UserRelativeIds.Any())
                                {
                                    WarrantyUserResponse[] warrantyUserResponse = warrantyRelativeUser.Data.Where(c => c.WarrantyId == request.Id && !request.UserRelativeIds.Contains(c.UserId))
                                                                               .ToArray();
                                    userRelativeIds.AddRange(warrantyUserResponse.Select(c => c.UserId));
                                    foreach (var item in warrantyUserResponse)
                                    {
                                        await WriteRepository.ExecuteAsync(SqlQuery.WARRANTY_USER_DELETE,
                                                                           new
                                                                           {
                                                                               WarrantyId = request.Id,
                                                                               UserId = item.UserId,
                                                                               Type = 1
                                                                           });
                                    }

                                    foreach (var user in request.UserRelativeIds)
                                    {
                                        var rs = await WriteRepository.ExecuteAsync(SqlQuery.WARRANTY_USER_INSERT,
                                                                                    new
                                                                                    {
                                                                                        WarrantyId = request.Id,
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
                                    if (warrantyRelativeUser.IsSuccess)
                                    {
                                        foreach (var item in warrantyRelativeUser.Data)
                                        {
                                            var rs = await WriteRepository.ExecuteAsync(SqlQuery.WARRANTY_USER_DELETE,
                                                                                        new
                                                                                        {
                                                                                            WarrantyId = request.Id,
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
                                    var rs = await WriteRepository.ExecuteAsync(SqlQuery.WARRANTY_USER_INSERT,
                                                                                new
                                                                                {
                                                                                    WarrantyId = request.Id,
                                                                                    UserId = user,
                                                                                    Type = 0,
                                                                                    UserUpdated = userId,
                                                                                    UserCreated = userId,
                                                                                    DateUpdated = DateTime.Now,
                                                                                    DateCreated = DateTime.Now
                                                                                });
                                }
                            }
                            await _busPublisher.SendEventAsync(new WarrantyUpdateSuccessEvent
                            {
                                Id = oldInformation.Data.Id,
                                Code = oldInformation.Data.Code,
                                BeforeUpdated = new WarrantyUpdateSuccessModel
                                {
                                    WarrantyStatusId = oldInformation.Data.WarrantyStatusId,
                                    DeliveryDate = oldInformation.Data.DeliveryDate,
                                    Description = oldInformation.Data.Description,
                                    UserCareIds = userCareIds,
                                    UserRelativeIds = userRelativeIds
                                },
                                AfterUpdated = new WarrantyUpdateSuccessModel
                                {
                                    WarrantyStatusId = oldInformation.Data.WarrantyStatusId,
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

        private async Task<TResponse<GetWarrantyByIdResponse>> CanUpdate(int userId,
                                                                    UpdateWarrantyRequest request)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if (checkPermission.IsSuccess)
                {
                    if (request.Id == 0)
                    {
                        return await Fail<GetWarrantyByIdResponse>(ErrorEnum.CRM_HAS_NOT_EXIST.GetStringValue());
                    }

                    var checkIdInvalid = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetWarrantyByIdResponse>(SqlQuery.WARRANTY_GET_BY_ID,
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

                        return await Fail<GetWarrantyByIdResponse>(ErrorEnum.CRM_HAS_NOT_EXIST.GetStringValue());
                    }

                    return await Fail<GetWarrantyByIdResponse>(checkIdInvalid.Message);
                }

                return await Fail<GetWarrantyByIdResponse>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetWarrantyByIdResponse>(exception);
            }
        }
        //TODO
        public async Task<TResponse<bool>> Comment(int userId, CommentWarrantyRequest request)
        {
            try
            {
                var warranty = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetWarrantyByIdResponse>(SqlQuery.WARRANTY_GET_BY_ID,
                                                                                                new
                                                                                                {
                                                                                                    request.Id
                                                                                                });
                if (warranty.IsSuccess)
                {
                    if (warranty.Data != null)
                    {
                        var rs1 = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.STATUS_WARRANTY_UPDATE, new { request.Id, request.WarrantyStatusId });
                        if (rs1.IsSuccess)
                        {
                            var rs = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.WARRANTY_INSERT_COMMENT,
                                                                               new
                                                                               {
                                                                                   WarrantyId = request.Id,
                                                                                   Comment = string.IsNullOrEmpty(request.Comment)
                                                                                                         ? "đã đính kèm file"
                                                                                                         : request.Comment,
                                                                                   UserId = userId,
                                                                                   warranty.Data.WarrantyStatusId,
                                                                                   OldWarrantyStatusId = warranty.Data.WarrantyStatusId
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

                                            var result = await WriteRepository.ExecuteAsync(SqlQuery.WARRANTY_LOG_ATTACHMENT_INSERT,
                                                                                            new
                                                                                            {
                                                                                                WarrantyId = crmLogId,
                                                                                                Attachment = $"{newFileName}{Path.GetExtension(file.FileName)}",
                                                                                                AttachmentName = file.FileName,
                                                                                                UserCreated = userId,
                                                                                                UserUpdated = userId
                                                                                            });
                                        }
                                    }

                                    await _busPublisher.SendEventAsync(new WarrantyCommentSuccessEvent
                                    {
                                        Id = request.Id,
                                        Code = warranty.Data.Code,
                                        Comment = request.Comment,
                                        HasAttachmentFile = request.Files != null && request.Files.Any(),
                                        UserId = userId,
                                        WarrantyStatusId = request.WarrantyStatusId
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

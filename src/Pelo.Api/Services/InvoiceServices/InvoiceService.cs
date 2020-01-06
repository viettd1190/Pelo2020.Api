﻿using System;
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
    }

    public class InvoiceService : BaseService,
                                  IInvoiceService
    {
        private readonly IAppConfigService _appConfigService;

        private readonly IRoleService _roleService;

        private readonly IUserService _userService;

        public InvoiceService(IDapperReadOnlyRepository readOnlyRepository,
                              IDapperWriteRepository writeRepository,
                              IHttpContextAccessor context,
                              IRoleService roleService,
                              IUserService userService,
                              IAppConfigService appConfigService) : base(readOnlyRepository,
                                                                         writeRepository,
                                                                         context)
        {
            _roleService = roleService;
            _appConfigService = appConfigService;
            _userService = userService;
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
                whereCondition = "AND ";
            }

            if(!string.IsNullOrEmpty(request.CustomerName))
            {
                whereBuilder.AppendFormat("{0}ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @CustomerName COLLATE Latin1_General_CI_AI",
                                          whereCondition);
                whereCondition = "AND ";
            }

            if(!string.IsNullOrEmpty(request.CustomerPhone))
            {
                whereBuilder.AppendFormat("{0}(ISNULL(c.Phone, '') LIKE @CustomerPhone OR ISNULL(c.Phone2, '') LIKE @CustomerPhone OR ISNULL(c.Phone3, '') LIKE @CustomerPhone)",
                                          whereCondition);
                whereCondition = "AND ";
            }

            if(!string.IsNullOrEmpty(request.Code))
            {
                whereBuilder.AppendFormat("{0}i.Code LIKE @Code",
                                          whereCondition);
                whereCondition = "AND ";
            }

            if(request.BranchId > 0)
            {
                whereBuilder.AppendFormat("{0}ISNULL(i.BranchId, 0) = @BranchId",
                                          whereCondition);
                whereCondition = "AND ";
            }

            if(request.InvoiceStatusId > 0)
            {
                whereBuilder.AppendFormat("{0}ISNULL(i.InvoiceStatusId, 0) = @InvoiceStatusId",
                                          whereCondition);
                whereCondition = "AND ";
            }

            var isDefaultInvoiceRoles = await _userService.IsBelongDefaultInvoiceRole(userId);
            if(!isDefaultInvoiceRoles.IsSuccess)
            {
                whereBuilder.AppendFormat("{0}(i.UserCreatedId = @UserCreatedId OR i.UserSellId = @UserSellId OR uii.UserId = @UserDeliveryId)",
                                          whereCondition);
                whereCondition = "AND ";
            }
            else
            {
                if(request.UserCreatedId > 0)
                {
                    whereBuilder.AppendFormat("{0}i.UserCreatedId = @UserCreatedId",
                                              whereCondition);
                    whereCondition = "AND ";
                }

                if(request.UserSellId > 0)
                {
                    whereBuilder.AppendFormat("{0}i.UserSellId = @UserSellId",
                                              whereCondition);
                    whereCondition = "AND ";
                }

                if(request.UserDeliveryId > 0)
                {
                    whereBuilder.AppendFormat("{0}uii.UserId = @UserDeliveryId AND uii.Type = 0",
                                              whereCondition);
                    whereCondition = "AND ";
                }
            }

            if(request.FromDate != null)
            {
                whereBuilder.AppendFormat("{0}i.DateCreated >= @FromDate",
                                          whereCondition);
                whereCondition = "AND ";
            }

            if(request.ToDate != null)
            {
                whereBuilder.AppendFormat("{0}i.DateCreated <= @ToDate",
                                          whereCondition);
                whereCondition = "AND ";
            }

            whereBuilder.AppendFormat("{0}i.IsDeleted = 0 AND c.IsDeleted = 0 AND uii.IsDeleted = 0",
                                      whereCondition);

            if(!string.IsNullOrEmpty(whereBuilder.ToString()))
            {
                sqlBuilder.AppendFormat("WHERE {0} ",
                                        whereBuilder);
            }

            sqlBuilder.Append(@"SELECT i.Id,
                                       i.Code,
                                       ins.Name AS InvoiceStatus,
                                       ins.Color AS InvoiceStatusColor,
                                       c.Name AS Customer,
                                       c.Phone AS CustomerPhone,
                                       c.Phone2 AS CustomerPhone2,
                                       c.Phone3 AS CustomerPhone3,
                                       c.Address,
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
    }
}

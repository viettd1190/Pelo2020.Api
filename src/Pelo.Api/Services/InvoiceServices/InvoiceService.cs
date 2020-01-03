using System;
using System.Collections.Generic;
using System.Linq;
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
    }

    public class InvoiceService : BaseService,
                                  IInvoiceService
    {
        private readonly IAppConfigService _appConfigService;

        private readonly IRoleService _roleService;

        public InvoiceService(IDapperReadOnlyRepository readOnlyRepository,
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
                        var crmUserCare = await ReadOnlyRepository.QueryAsync<UserDisplaySimpleModel>(SqlQuery.INVOICE_USER_DELIVERY_GET_BY_CRM_ID,
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

        #endregion
    }
}

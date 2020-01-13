using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Warranty;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.WarrantyServices
{
    public interface IWarrantyService
    {
        //Task<PageResult<GetWarrantyPagingResponse>> GetByCustomerId(int userId, int customerId, int page, int pageSize);
    }
    public class WarrantyService : BaseService, IWarrantyService
    {
        private readonly IAppConfigService _appConfigService;

        private readonly IRoleService _roleService;

        private readonly IUserService _userService;

        private IProductService _productService;
        public WarrantyService(IDapperReadOnlyRepository readOnlyRepository, IDapperWriteRepository writeRepository, IHttpContextAccessor context, IRoleService roleService,
                              IUserService userService,
                              IProductService productService,
                              IAppConfigService appConfigService) : base(readOnlyRepository, writeRepository, context)
        {
            _roleService = roleService;
            _appConfigService = appConfigService;
            _userService = userService;
            _productService = productService;
        }

        //public async Task<PageResult<GetWarrantyPagingResponse>> GetByCustomerId(int userId, int customerId, int page, int pageSize)
        //{
        //    try
        //    {
        //        var result = new TResponse<(PageResult<GetWarrantyPagingResponse>, int)>();

        //        bool canGetAll = false;

        //        var canGetAllCrm = await _appConfigService.GetByName("DefaultCRMAcceptRoles");
        //        if (canGetAllCrm.IsSuccess)
        //        {
        //            var defaultRoles = canGetAllCrm.Data.Split(" ");
        //            var currentRole = await _roleService.GetNameByUserId(userId);
        //            if (currentRole.IsSuccess
        //               && !string.IsNullOrEmpty(currentRole.Data)
        //               && defaultRoles.Contains(currentRole.Data))
        //            {
        //                canGetAll = true;
        //            }
        //        }

        //        if (canGetAll)
        //        {
        //            result = await ReadOnlyRepository.QueryMultipleLFAsync<GetWarrantyPaginResponse, int>(SqlQuery.CRM_GET_BY_CUSTOMER_ID,
        //                                                                                              new
        //                                                                                              {
        //                                                                                                  CustomerId = customerId,
        //                                                                                                  Skip = (page - 1) * pageSize,
        //                                                                                                  Take = pageSize
        //                                                                                              });
        //        }
        //        else
        //        {
        //            result = await ReadOnlyRepository.QueryMultipleLFAsync<GetWarrantyPaginResponse, int>(SqlQuery.CRM_GET_BY_CUSTOMER_ID_2,
        //                                                                                              new
        //                                                                                              {
        //                                                                                                  CustomerId = customerId,
        //                                                                                                  UserCareId = userId,
        //                                                                                                  Skip = (page - 1) * pageSize,
        //                                                                                                  Take = pageSize
        //                                                                                              });
        //        }

        //        if (result.IsSuccess)
        //        {
        //            //foreach (var crm in result.Data.Item1)
        //            //{
        //            //    crm.UserCares = new List<UserDisplaySimpleModel>();
        //            //    var crmUserCare = await ReadOnlyRepository.Query<UserDisplaySimpleModel>(SqlQuery.CRM_USER_CARE_GET_BY_CRM_ID,
        //            //                                                                             new
        //            //                                                                             {
        //            //                                                                                 CrmId = crm.Id
        //            //                                                                             });
        //            //    if (crmUserCare.IsSuccess
        //            //       && crmUserCare.Data != null)
        //            //    {
        //            //        crm.UserCares.AddRange(crmUserCare.Data);
        //            //    }
        //            //}

        //            return await Ok(new PageResult<GetWarrantyPagingResponse>(page,
        //                                                                 pageSize,
        //                                                                 result.Data.Item2,
        //                                                                 result.Data.Item1));
        //        }

        //        return await Fail<PageResult<GetWarrantyPagingResponse>>(result.Message);
        //    }
        //    catch (Exception exception)
        //    {
        //        return await Fail<PageResult<GetWarrantyPagingResponse>>(exception);
        //    }
        //}
    }
}

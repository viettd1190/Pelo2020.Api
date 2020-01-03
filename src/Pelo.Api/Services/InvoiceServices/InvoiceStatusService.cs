using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.InvoiceStatus;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.InvoiceServices
{
    public interface IInvoiceStatusService
    {
        Task<TResponse<IEnumerable<InvoiceStatusSimpleModel>>> GetAll(int userId);
    }

    public class InvoiceStatusService : BaseService,
                                        IInvoiceStatusService
    {
        private readonly IRoleService _roleService;

        public InvoiceStatusService(IDapperReadOnlyRepository readOnlyRepository,
                                    IDapperWriteRepository writeRepository,
                                    IHttpContextAccessor context,
                                    IRoleService roleService) : base(readOnlyRepository,
                                                                     writeRepository,
                                                                     context)
        {
            _roleService = roleService;
        }

        #region IInvoiceStatusService Members

        public async Task<TResponse<IEnumerable<InvoiceStatusSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if(canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<InvoiceStatusSimpleModel>(SqlQuery.INVOICE_STATUS_GET_ALL);
                    if(result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<InvoiceStatusSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<InvoiceStatusSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<InvoiceStatusSimpleModel>>(exception);
            }
        }

        #endregion

        private async Task<TResponse<bool>> CanGetAll(int userId)
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

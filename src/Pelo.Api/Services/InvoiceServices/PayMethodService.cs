using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.PayMethod;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.InvoiceServices
{
    public interface IPayMethodService
    {
        Task<TResponse<IEnumerable<PayMethodSimpleModel>>> GetAll(int userId);
    }

    public class PayMethodService : BaseService,
                                    IPayMethodService
    {
        private readonly IRoleService _roleService;

        public PayMethodService(IDapperReadOnlyRepository readOnlyRepository,
                                IDapperWriteRepository writeRepository,
                                IHttpContextAccessor context,
                                IRoleService roleService) : base(readOnlyRepository,
                                                                 writeRepository,
                                                                 context)
        {
            _roleService = roleService;
        }

        #region IPayMethodService Members

        public async Task<TResponse<IEnumerable<PayMethodSimpleModel>>> GetAll(int userId)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if(canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryAsync<PayMethodSimpleModel>(SqlQuery.PAY_METHOD_GET_ALL);
                    if(result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<IEnumerable<PayMethodSimpleModel>>(result.Message);
                }

                return await Fail<IEnumerable<PayMethodSimpleModel>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<PayMethodSimpleModel>>(exception);
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

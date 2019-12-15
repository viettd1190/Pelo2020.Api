using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Common.Dtos.Ward;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IWardService
    {
        Task<TResponse<IEnumerable<WardModel>>> GetAll(int districtId);
    }

    public class WardService : BaseService,
                                   IWardService
    {
        public WardService(IDapperReadOnlyRepository readOnlyRepository,
                               IDapperWriteRepository writeRepository,
                               IHttpContextAccessor context) : base(readOnlyRepository,
                                                                    writeRepository,
                                                                    context)
        {
        }

        #region IWardService Members

        public async Task<TResponse<IEnumerable<WardModel>>> GetAll(int districtId)
        {
            try
            {
                var result = await ReadOnlyRepository.Query<WardModel>(SqlQuery.WARD_GET_ALL,
                                                                           new
                                                                           {
                                                                                   DistrictId = districtId
                                                                           });
                if(result.IsSuccess)
                {
                    return await Ok(result.Data);
                }

                return await Fail<IEnumerable<WardModel>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<WardModel>>(exception);
            }
        }

        #endregion
    }
}

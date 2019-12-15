using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Common.Dtos.District;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IDistrictService
    {
        Task<TResponse<IEnumerable<DistrictModel>>> GetAll(int provinceId);
    }

    public class DistrictService : BaseService,
                                   IDistrictService
    {
        public DistrictService(IDapperReadOnlyRepository readOnlyRepository,
                               IDapperWriteRepository writeRepository,
                               IHttpContextAccessor context) : base(readOnlyRepository,
                                                                    writeRepository,
                                                                    context)
        {
        }

        #region IDistrictService Members

        public async Task<TResponse<IEnumerable<DistrictModel>>> GetAll(int provinceId)
        {
            try
            {
                var result = await ReadOnlyRepository.Query<DistrictModel>(SqlQuery.DISTRICT_GET_ALL,
                                                                           new
                                                                           {
                                                                                   ProvinceId = provinceId
                                                                           });
                if(result.IsSuccess)
                {
                    return await Ok(result.Data);
                }

                return await Fail<IEnumerable<DistrictModel>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<DistrictModel>>(exception);
            }
        }

        #endregion
    }
}

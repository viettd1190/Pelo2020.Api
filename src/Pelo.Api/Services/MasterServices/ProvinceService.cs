using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Common.Dtos.Province;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.MasterServices
{
    public interface IProvinceService
    {
        Task<TResponse<IEnumerable<ProvinceModel>>> GetAll();
    }

    public class ProvinceService : BaseService,
                                   IProvinceService
    {
        public ProvinceService(IDapperReadOnlyRepository readOnlyRepository,
                               IDapperWriteRepository writeRepository,
                               IHttpContextAccessor context) : base(readOnlyRepository,
                                                                    writeRepository,
                                                                    context)
        {
        }

        #region IProvinceService Members

        public async Task<TResponse<IEnumerable<ProvinceModel>>> GetAll()
        {
            try
            {
                var result = await ReadOnlyRepository.Query<ProvinceModel>(SqlQuery.PROVINCE_GET_ALL);
                if(result.IsSuccess)
                {
                    return await Ok(result.Data);
                }

                return await Fail<IEnumerable<ProvinceModel>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<ProvinceModel>>(exception);
            }
        }

        #endregion
    }
}

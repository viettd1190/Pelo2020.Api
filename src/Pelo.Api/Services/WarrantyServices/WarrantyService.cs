using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.WarrantyServices
{
    public interface IWarrantService
    {

    }
    public class WarrantyService : BaseService, IWarrantService
    {
        public WarrantyService(IDapperReadOnlyRepository readOnlyRepository, IDapperWriteRepository writeRepository, IHttpContextAccessor context) : base(readOnlyRepository, writeRepository, context)
        {
        }
    }
}

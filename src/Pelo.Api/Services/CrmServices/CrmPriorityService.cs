using Pelo.Api.Services.BaseServices;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.CrmServices
{
    public interface ICrmPriorityService
    {
    }

    public class CrmPriorityService : BaseService, ICrmPriorityService
    {
        public CrmPriorityService(IDapperReadOnlyRepository readOnlyRepository, IDapperWriteRepository writeRepository)
            : base(readOnlyRepository, writeRepository)
        {
        }
    }
}
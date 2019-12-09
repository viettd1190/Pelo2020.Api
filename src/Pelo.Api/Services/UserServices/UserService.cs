using Pelo.Api.Services.BaseServices;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.UserServices
{
    public interface IUserService
    {
    }

    public class UserService : BaseService, IUserService
    {
        public UserService(IDapperReadOnlyRepository readOnlyRepository, IDapperWriteRepository writeRepository) : base(
            readOnlyRepository, writeRepository)
        {
        }
    }
}
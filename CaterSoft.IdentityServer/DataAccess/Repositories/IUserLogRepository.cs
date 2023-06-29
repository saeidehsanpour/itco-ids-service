using System.Threading.Tasks;
using CaterSoft.IdentityServer.DataAccess.Model;

namespace CaterSoft.IdentityServer.DataAccess.Repositories
{
    public interface IUserLogRepository
    {
        Task Add(UserLog userLog);
    }
}
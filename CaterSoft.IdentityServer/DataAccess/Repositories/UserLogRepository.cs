using System.Threading.Tasks;
using CaterSoft.IdentityServer.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace CaterSoft.IdentityServer.DataAccess.Repositories
{
    public class UserLogRepository : IUserLogRepository
    {
        public readonly DbContext _dbContext;
        public UserLogRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(UserLog userLog)
        {
            _dbContext.Set<UserLog>().Add(userLog);
            await _dbContext.SaveChangesAsync();
        }
    }
}

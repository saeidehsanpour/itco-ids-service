using CaterSoft.IdentityServer.DataAccess.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CaterSoft.IdentityServer.DataAccess
{
    public class IdentityContext : IdentityDbContext<UserIdentity, RoleIdentity, string,
        IdentityUserClaim<string>, UserRoleIdentity, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public IConfiguration _configuration { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public IdentityContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring
            (DbContextOptionsBuilder optionsBuilder) =>
                optionsBuilder.UseNpgsql(_configuration.GetConnectionString("sqlConnection"));
    }
}

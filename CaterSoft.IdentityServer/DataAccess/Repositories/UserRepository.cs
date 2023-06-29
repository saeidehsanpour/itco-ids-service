using System;
using System.Collections.Generic;
using System.Linq;
using CaterSoft.IdentityServer.DataAccess.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CaterSoft.IdentityServer.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly DbContext _dbContext;
        public UserRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public UserIdentity Find(string userName)
        {
            return _dbContext.Set<UserIdentity>().FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower());
        }
        public UserIdentity Find(Guid UserId)
        {
            return _dbContext.Set<UserIdentity>().FirstOrDefault(x => x.Id == UserId.ToString());
        }
        public List<string> FindRoles(string userId)
        {
            var roles = (from ur in _dbContext.Set<UserRoleIdentity>()
                         join role in _dbContext.Set<RoleIdentity>() on ur.RoleId equals role.Id
                         where ur.UserId == userId
                         select role.Name).ToList();
            return roles;
        }
        public List<ClaimResponse> FindClaims(string userId)
        {
            var claimResponses = (from userClaimIdentity in _dbContext.Set<IdentityUserClaim<string>>()
                where userClaimIdentity.UserId == userId
                         select new ClaimResponse
                         {
                             ClaimType = userClaimIdentity.ClaimType,
                             ClaimValue = userClaimIdentity.ClaimValue
                         }).ToList();
            return claimResponses;
        }
    }
}

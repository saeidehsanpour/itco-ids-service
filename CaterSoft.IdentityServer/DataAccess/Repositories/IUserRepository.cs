using System;
using System.Collections.Generic;
using CaterSoft.IdentityServer.DataAccess.Model;

namespace CaterSoft.IdentityServer.DataAccess.Repositories
{
    public interface IUserRepository
    {
        UserIdentity Find(string userName);
        UserIdentity Find(Guid userId);
        List<string> FindRoles(string userId);
        List<ClaimResponse> FindClaims(string userId);
    }
}
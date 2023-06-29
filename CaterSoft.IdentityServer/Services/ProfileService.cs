using System;
using System.Linq;
using System.Threading.Tasks;
using CaterSoft.IdentityServer.DataAccess.Model;
using CaterSoft.IdentityServer.DataAccess.Repositories;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

namespace CaterSoft.IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;

        public ProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                if (!string.IsNullOrEmpty(context.Subject.Identity.Name))
                {
                    var userNameClaim = context.Subject.Claims.FirstOrDefault(a => a.Type == IdentityModel.JwtClaimTypes.PreferredUserName);
                    var user = _userRepository.Find(userNameClaim != null ? userNameClaim.Value : context.Subject.Identity.Name);

                    if (user != null)
                    {
                        var roles = _userRepository.FindRoles(user.Id);
                        var claimsRes = _userRepository.FindClaims(user.Id);
                        var claims = ResourceOwnerPasswordValidator.GetUserClaims(user, roles, claimsRes);

                        context.IssuedClaims = claims.Where(x =>
                            context.RequestedClaimTypes.Contains(x.Type) ||
                            x.Type == IdentityModel.JwtClaimTypes.PreferredUserName ||
                            x.Type == IdentityModel.JwtClaimTypes.Name ||
                            x.Type == IdentityModel.JwtClaimTypes.Email ||
                            x.Type == IdentityModel.JwtClaimTypes.GivenName ||
                            x.Type == IdentityModel.JwtClaimTypes.Role).ToList();
                    }
                }
                else
                {
                    var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");

                    if (Guid.Parse(userId?.Value) != default)
                    {
                        var user = _userRepository.Find(Guid.Parse(userId?.Value));

                        if (user != null)
                        {
                            var roles = _userRepository.FindRoles(user.Id);
                            var claimsRes = _userRepository.FindClaims(user.Id);
                            var claims = ResourceOwnerPasswordValidator.GetUserClaims(user, roles, claimsRes);

                            context.IssuedClaims = claims.Where(x =>
                                context.RequestedClaimTypes.Contains(x.Type) ||
                                x.Type == IdentityModel.JwtClaimTypes.PreferredUserName ||
                                x.Type == IdentityModel.JwtClaimTypes.Name ||
                                x.Type == IdentityModel.JwtClaimTypes.Email ||
                                x.Type == IdentityModel.JwtClaimTypes.Role).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //log your error
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            try
            {
                var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");

                if (Guid.Parse(userId.Value) != default)
                {
                    var user = _userRepository.Find(Guid.Parse(userId.Value));

                    if (user?.IsActive ?? false)
                    {
                        context.IsActive = user.IsActive;
                    }
                }
            }
            catch (Exception)
            {
                //handle error logging
            }
        }
    }
}
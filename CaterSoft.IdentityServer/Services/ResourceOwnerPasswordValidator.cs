using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using CaterSoft.IdentityServer.DataAccess.Model;
using CaterSoft.IdentityServer.DataAccess.Repositories;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Http;
using UAParser;

namespace CaterSoft.IdentityServer.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserLogRepository _userLogRepository;

        public ResourceOwnerPasswordValidator(IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor, IUserLogRepository userLogRepository)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _userLogRepository = userLogRepository;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                //
                var user = _userRepository.Find(context.UserName);
                if (user is { IsActive: true })
                {
                    if (user.PasswordHash == HashPassword(context.Password))
                    {
                        var _customResponse = new Dictionary<string, object>
                        {
                            {"fullname", user.FullName}
                        };

                        context.Result = new GrantValidationResult(
                            subject: user.Id,
                            authenticationMethod: "custom",
                            claims: GetUserClaims(user),
                            customResponse: _customResponse);

                        await RegisterLog(user.Id);

                        return;
                    }
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Username or password is wrong");
                    return;
                }
                context.Result = new GrantValidationResult(TokenRequestErrors.UnauthorizedClient, "Username or password is wrong");
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ex.Message);
            }
        }

        public static Claim[] GetUserClaims(UserIdentity user, List<string> roles = null, List<ClaimResponse> claimResponses = null)
        {
            var claims = new List<Claim>
            {
                new Claim("user_id", user.Id.ToString() ?? ""),
                new Claim(JwtClaimTypes.Name, (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName)) ? (user.FirstName + " " + user.LastName) : user.FullName),
                new Claim(JwtClaimTypes.FamilyName, user.LastName  ?? ""),
                new Claim(JwtClaimTypes.Email, user.Email  ?? ""),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
            };

            if (roles?.Any() ?? false)
                claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

            if (claimResponses?.Any() ?? false)
                claims.AddRange(claimResponses.Select(s => new Claim(s.ClaimType, s.ClaimValue)));
            return claims.ToArray();
        }

        public static string HashPassword(string password)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(password))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private async Task RegisterLog(string userId)
        {
            var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
            var platform = _httpContextAccessor.HttpContext.Request.Headers["platform"].ToString();

            var parser = Parser.GetDefault();
            ClientInfo client = parser.Parse(userAgent);

            UserLog userLog = new UserLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Device = client.Device.Family,
                Platform = platform,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            await _userLogRepository.Add(userLog);
        }
    }
}

using BudgetCast.Identity.Api.ApiModels.Token;
using BudgetCast.Identity.Api.Database.Models;
using BudgetCast.Identity.Api.Infrastructure.AppSettings;
using BudgetCast.Identity.Api.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BudgetCast.Identity.Api.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<TokenService> _localizer;
        private readonly JwtSettings _jwtSettings;

        public TokenService(
            UserManager<ApplicationUser> userManager, 
            IStringLocalizer<TokenService> localizer, 
            JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _localizer = localizer;
            _jwtSettings = jwtSettings;
        }

        public string GenerateAccessToken(ApplicationUser user, string ipAddress)
        {
            return GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, ipAddress));
        }

        public TokenResponseVm GetToken(ApplicationUser user, string ipAddress)
        {
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
            var token = GenerateAccessToken(user, ipAddress);
            var response = new TokenResponseVm(token, refreshToken, refreshTokenExpiryTime);
            return response;
        }

        public async Task<TokenResponseVm> GetTokenAsync(TokenRequestDto request, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize());
            if (user == null)
            {
                throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.Unauthorized);
            }

            if (!user.IsActive)
            {
                throw new IdentityException(_localizer["identity.usernotactive"], statusCode: HttpStatusCode.Unauthorized);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new IdentityException(_localizer["identity.emailnotconfirmed"], statusCode: HttpStatusCode.Unauthorized);
            }

            bool passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                throw new IdentityException(_localizer["identity.invalidcredentials"], statusCode: HttpStatusCode.Unauthorized);
            }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
            await _userManager.UpdateAsync(user);

            var token = GenerateAccessToken(user, ipAddress);
            var response = new TokenResponseVm(token, user.RefreshToken, user.RefreshTokenExpiryTime);
            return response;
        }

        public async Task<TokenResponseVm> RefreshTokenAsync(RefreshTokenRequestDto request, string ipAddress)
        {
            if (request is null)
            {
                throw new IdentityException(_localizer["identity.invalidtoken"], statusCode: HttpStatusCode.Unauthorized);
            }

            var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
            string userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.NotFound);
            }

            if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new IdentityException(_localizer["identity.invalidrefreshtoken"], statusCode: HttpStatusCode.Unauthorized);
            }

            string token = GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, ipAddress));
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
            await _userManager.UpdateAsync(user);
            var response = new TokenResponseVm(token, user.RefreshToken, user.RefreshTokenExpiryTime);
            return response;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrEmpty(_jwtSettings.Key))
            {
                throw new InvalidOperationException("No Key defined in JwtSettings config.");
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),

                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,

                ValidateLifetime = true,

                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.Name,

                ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new IdentityException(_localizer["identity.invalidtoken"], statusCode: HttpStatusCode.Unauthorized);
            }

            return principal;
        }

        private static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
                signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }        

        private SigningCredentials GetSigningCredentials()
        {
            if (string.IsNullOrEmpty(_jwtSettings.Key))
            {
                throw new InvalidOperationException("No Key defined in JwtSettings config.");
            }

            byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        private IEnumerable<Claim> GetClaims(ApplicationUser user, string ipAddress)
        {
            return new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimConstants.Fullname, $"{user.GivenName} {user.LastName}"),
                new(ClaimTypes.Name, user.GivenName ?? string.Empty),
                new(ClaimTypes.GivenName, user.GivenName ?? string.Empty),
                new(ClaimTypes.Surname, user.LastName ?? string.Empty),
                new(ClaimConstants.IpAddress, ipAddress),
                new(ClaimConstants.Tenant, "7064"),
                new(ClaimConstants.ImageUrl, user.ImageUrl ?? string.Empty),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            };
        }
    }
}

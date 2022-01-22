using BudgetCast.Identity.Api.ApiModels.Token;
using BudgetCast.Identity.Api.Database.Models;

namespace BudgetCast.Identity.Api.Infrastructure.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user, string ipAddress);

        TokenResponseVm GetToken(ApplicationUser user, string ipAddress);

        Task<TokenResponseVm> GetTokenAsync(TokenRequestDto request, string ipAddress);

        Task<TokenResponseVm> RefreshTokenAsync(RefreshTokenRequestDto request, string ipAddress);
    }
}
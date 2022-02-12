using BudgetCast.Identity.Api.ApiModels.Token;
using BudgetCast.Identity.Api.Database.Models;
using System.Security.Claims;

namespace BudgetCast.Identity.Api.Infrastructure.Services
{
    public interface ITokenService
    {
        TokenResponseVm GetToken(ApplicationUser user, string ipAddress);

        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);        
    }
}
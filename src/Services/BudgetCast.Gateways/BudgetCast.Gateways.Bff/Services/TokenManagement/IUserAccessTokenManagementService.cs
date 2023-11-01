using System.Security.Claims;

namespace BudgetCast.Gateways.Bff.Services.TokenManagement;

public interface IUserAccessTokenManagementService
{
    Task<string> GetUserAccessTokenAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);
}
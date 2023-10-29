using System.Security.Claims;

namespace BudgetCast.Gateways.Bff.Services;

public interface IUserAccessTokenManagementService
{
    Task<string> GetUserAccessTokenAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);
}
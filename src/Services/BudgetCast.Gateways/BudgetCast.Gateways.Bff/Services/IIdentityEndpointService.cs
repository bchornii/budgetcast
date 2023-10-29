using BudgetCast.Gateways.Bff.Models;

namespace BudgetCast.Gateways.Bff.Services;

public interface IIdentityEndpointService
{
    Task<(bool IsSuccess, int StatusCode, string AccessToken)> LoginAsync(LoginDto dto,
        CancellationToken cancellationToken);

    Task<(bool IsSuccess, int StatusCode)> LogoutAsync();

    Task<(bool IsSuccess, int StatusCode, string NewAccessToken)> RefreshAccessTokenAsync(RefreshTokenDto dto,
        CancellationToken cancellationToken);
}
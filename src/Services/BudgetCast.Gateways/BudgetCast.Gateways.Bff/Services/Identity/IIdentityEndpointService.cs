using System.Net;
using BudgetCast.Gateways.Bff.Endpoints.Login;
using BudgetCast.Gateways.Bff.Models;

namespace BudgetCast.Gateways.Bff.Services.Identity;

#region Models

public record LoginResult(bool IsSuccess, HttpStatusCode StatusCode, string AccessToken);

public record LogoutResult(bool IsSuccess, HttpStatusCode StatusCode);

public record RefreshAccessTokenResult(bool IsSuccess, HttpStatusCode StatusCode, string NewAccessToken);

#endregion

public interface IIdentityEndpointService
{
    Task<LoginResult> LoginAsync(LoginDto dto, CancellationToken cancellationToken);

    Task<LogoutResult> LogoutAsync(string accessToken);

    Task<RefreshAccessTokenResult> RefreshAccessTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken);
}
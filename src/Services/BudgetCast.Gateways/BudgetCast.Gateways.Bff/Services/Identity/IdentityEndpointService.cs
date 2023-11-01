using System.Net;
using System.Net.Http.Headers;
using BudgetCast.Gateways.Bff.Endpoints.Login;
using BudgetCast.Gateways.Bff.Models;

namespace BudgetCast.Gateways.Bff.Services.Identity;

public class IdentityEndpointService : IIdentityEndpointService
{
    private readonly HttpClient _httpClient;

    public IdentityEndpointService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResult> LoginAsync(
        LoginDto dto, CancellationToken cancellationToken)
    {
        var responseMessage = await _httpClient
            .PostAsJsonAsync("/api/signin/individual", dto, cancellationToken);

        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            var result = await responseMessage.Content
                .ReadFromJsonAsync<LoginVm>(cancellationToken: cancellationToken);

            return new LoginResult(true, responseMessage.StatusCode, result!.AccessToken);
        }

        return new LoginResult(false, responseMessage.StatusCode, null!);
    }

    public async Task<LogoutResult> LogoutAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", accessToken);
        var responseMessage = await _httpClient
            .PostAsync("/api/signout", new StringContent(string.Empty));
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        return new LogoutResult(responseMessage.StatusCode == HttpStatusCode.OK, responseMessage.StatusCode);
    }

    public async Task<RefreshAccessTokenResult> RefreshAccessTokenAsync(RefreshTokenDto dto,
        CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", dto.AccessToken);
        var responseMessage = await _httpClient
            .PostAsJsonAsync("/api/signin/refresh", dto, cancellationToken);
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            var result = await responseMessage.Content
                .ReadFromJsonAsync<RefreshAccessTokenVm>(cancellationToken: cancellationToken);

            return new RefreshAccessTokenResult(true, responseMessage.StatusCode, result!.AccessToken);
        }

        return new RefreshAccessTokenResult(false, responseMessage.StatusCode, null!);
    }
}
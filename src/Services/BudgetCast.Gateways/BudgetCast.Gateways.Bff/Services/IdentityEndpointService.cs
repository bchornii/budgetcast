using System.Net;
using BudgetCast.Gateways.Bff.Models;

namespace BudgetCast.Gateways.Bff.Services;

public class IdentityEndpointService : IIdentityEndpointService
{
    private readonly HttpClient _httpClient;

    public IdentityEndpointService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public Task<(bool IsSuccess, RefreshAccessTokenVm)> RefreshUserAccessTokenAsync(string oldAccessToken)
    {
        throw new NotImplementedException();
    }

    public async Task<(bool IsSuccess, int StatusCode, string AccessToken)> LoginAsync(
        LoginDto dto, CancellationToken cancellationToken)
    {
        var responseMessage = await _httpClient
            .PostAsJsonAsync("/api/signin/individual", dto, cancellationToken);

        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            var result = await responseMessage.Content
                .ReadFromJsonAsync<LoginVm>(cancellationToken: cancellationToken);

            return (true, 200, result!.AccessToken);
        }

        return (false, (int)responseMessage.StatusCode, null!);
    }

    public async Task<(bool IsSuccess, int StatusCode)> LogoutAsync()
    {
        var responseMessage = await _httpClient
            .PostAsync("/api/signout", new StringContent(string.Empty));

        return (responseMessage.StatusCode == HttpStatusCode.OK, (int)responseMessage.StatusCode);
    }

    public async Task<(bool IsSuccess, int StatusCode, string NewAccessToken)> RefreshAccessTokenAsync(
        RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {dto.AccessToken}");
        
        var responseMessage = await _httpClient
            .PostAsJsonAsync("/api/signin/refresh", dto, cancellationToken);
        
        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            var result = await responseMessage.Content
                .ReadFromJsonAsync<RefreshAccessTokenVm>(cancellationToken: cancellationToken);

            return (true, 200, result!.AccessToken);
        }

        return (false, (int)responseMessage.StatusCode, null!);
    }
}
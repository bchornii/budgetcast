using System.Net;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Gateways.Bff.Extensions;
using BudgetCast.Gateways.Bff.Models;
using BudgetCast.Gateways.Bff.Services.Identity;
using BudgetCast.Gateways.Bff.Services.TokenStore;
using Microsoft.AspNetCore.Authentication;

namespace BudgetCast.Gateways.Bff.Endpoints.Logout;

public record LogoutIndividualRequest(HttpContext HttpContext) : EndpointRequest;

public class LogoutIndividualEndpoint : IEndpoint<LogoutIndividualRequest, IResult>
{
    private readonly BffOptions _bffOptions;
    private readonly IIdentityEndpointService _identityEndpointService;
    private readonly IUserAccessTokenStore _accessTokenStore;
    private readonly ILogger<LogoutIndividualEndpoint> _logger;

    public LogoutIndividualEndpoint(
        BffOptions bffOptions, 
        IIdentityEndpointService identityEndpointService,
        IUserAccessTokenStore accessTokenStore,
        ILogger<LogoutIndividualEndpoint> logger)
    {
        _bffOptions = bffOptions;
        _identityEndpointService = identityEndpointService;
        _accessTokenStore = accessTokenStore;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(LogoutIndividualRequest request, CancellationToken token)
    {
        var uuid = request.HttpContext.User.GetUuid();
        _logger.LogInformation("Remote sign out for {uuid}", uuid);
        var userAccessToken = await _accessTokenStore.GetTokenAsync(request.HttpContext.User);
        var result = await _identityEndpointService.LogoutAsync(userAccessToken.AccessToken);
        _logger.LogInformation("Remote sign out for {uuid} finished with {statusCode} status code", uuid, (int)result.StatusCode);
        
        _logger.LogInformation("Local sign out for {uuid}", uuid);
        await request.HttpContext.SignOutAsync(AppConstants.DefaultAppAuthScheme);

        return Results.NoContent();
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app
            .MapPost(_bffOptions.LogoutIndividualPath, async (
                    HttpContext context,
                    CancellationToken cancellationToken)
                => await HandleAsync(new LogoutIndividualRequest(context), cancellationToken))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization()
            .WithTags("logout-individual");
    }
}
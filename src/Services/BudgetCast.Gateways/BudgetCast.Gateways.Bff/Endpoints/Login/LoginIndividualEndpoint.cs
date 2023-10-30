using BudgetCast.Gateways.Bff.Extensions;
using BudgetCast.Gateways.Bff.Services.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Gateways.Bff.Endpoints.Login;

public record LoginIndividualRequest(LoginDto Dto, HttpContext HttpContext) : EndpointRequest;

public class LoginIndividualEndpoint : IEndpoint<LoginIndividualRequest, IResult>
{
    private readonly BffOptions _bffOptions;
    private readonly IIdentityEndpointService _identityEndpointService;

    public LoginIndividualEndpoint(BffOptions bffOptions, IIdentityEndpointService identityEndpointService)
    {
        _bffOptions = bffOptions;
        _identityEndpointService = identityEndpointService;
    }

    public async Task<IResult> HandleAsync(LoginIndividualRequest request, CancellationToken token)
    {
        var result = await _identityEndpointService.LoginAsync(request.Dto, token);

        if (!result.IsSuccess)
        {
            return Results.StatusCode((int)result.StatusCode);
        }
        
        if (!string.IsNullOrWhiteSpace(result.AccessToken))
        {
            await request.HttpContext
                .SignInAsCookieAsync(result.AccessToken);
        }
            
        return Results.NoContent();
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app
            .MapPost(_bffOptions.LoginIndividualPath, async (
                    [FromBody] LoginDto dto,
                    HttpContext context,
                    CancellationToken cancellationToken)
                => await HandleAsync(new LoginIndividualRequest(dto, context), cancellationToken))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("login-individual");
    }
}
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using BudgetCast.Gateways.Bff.Models;
using BudgetCast.Gateways.Bff.Services.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            var claims = new[]
            {
                new Claim("uuid", Guid.NewGuid().ToString("N")),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties();

            var expiration = new JwtSecurityTokenHandler()
                .ReadJwtToken(result.AccessToken).ValidTo;
            authProperties.Items[AppConstants.ExpiresAtName] = expiration.ToString("o", CultureInfo.InvariantCulture);
            authProperties.Items[AppConstants.AccessTokenName] = result.AccessToken;
            authProperties.Items[AppConstants.RefreshTokenName] = "Not-passed";

            await request.HttpContext.SignInAsync(
                AppConstants.DefaultAppAuthScheme, 
                principal,
                authProperties);
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
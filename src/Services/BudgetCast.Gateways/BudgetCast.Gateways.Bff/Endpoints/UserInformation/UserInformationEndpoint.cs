using System.Text.Encodings.Web;
using System.Text.Json;
using BudgetCast.Gateways.Bff.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace BudgetCast.Gateways.Bff.Endpoints.UserInformation;

public record ClaimRecord(string Type, object Value);

public record UserInformationRequest(HttpContext HttpContext) : EndpointRequest;

public class UserInformationEndpoint : IEndpoint<UserInformationRequest, IResult>
{
    private readonly BffOptions _bffOptions;

    public UserInformationEndpoint(BffOptions bffOptions)
    {
        _bffOptions = bffOptions;
    }

    public async Task<IResult> HandleAsync(UserInformationRequest request, CancellationToken token)
    {
        var result = await request.HttpContext.AuthenticateAsync(AppConstants.DefaultAppAuthScheme);

        if (!result.Succeeded)
        {
            return Results.Unauthorized();
        }
        
        var claims = new List<ClaimRecord>();
        claims.AddRange(GetUserClaims(result));
        claims.AddRange(GetManagementClaims(request.HttpContext, result));
        
        return Results.Ok(claims);
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app
            .MapGet(_bffOptions.UserPath, async (
                    HttpContext context,
                    CancellationToken cancellationToken)
                => await HandleAsync(new UserInformationRequest(context), cancellationToken))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization()
            .WithTags("user-information");
    }
    
    protected IEnumerable<ClaimRecord> GetUserClaims(AuthenticateResult authenticateResult) 
        => authenticateResult.Principal!.Claims.Select(x => new ClaimRecord(x.Type, x.Value));
    
    protected IEnumerable<ClaimRecord> GetManagementClaims(HttpContext context, AuthenticateResult authenticateResult)
    {
        var claims = new List<ClaimRecord>();
        
        var sessionId = authenticateResult.Principal!.FindFirst(JwtClaimTypes.SessionId)?.Value;
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            claims.Add(new ClaimRecord("SessionId", sessionId));
        }

        if (authenticateResult.Properties != null)
        {
            if (authenticateResult.Properties.ExpiresUtc.HasValue)
            {
                var expiresInSeconds = authenticateResult.Properties.ExpiresUtc.Value.Subtract(DateTimeOffset.UtcNow).TotalSeconds;
                var expiresInMinutes = expiresInSeconds / 60;
                var expiresInHours = expiresInMinutes / 60;
                var expiresInDays = expiresInHours / 24;
                
                claims.Add(new ClaimRecord("SessionExpiresInSec",Math.Round(expiresInSeconds)));   
                claims.Add(new ClaimRecord("SessionExpiresInMin",Math.Round(expiresInMinutes)));   
                claims.Add(new ClaimRecord("SessionExpiresInHours",Math.Round(expiresInHours)));   
                claims.Add(new ClaimRecord("SessionExpiresInDays",Math.Round(expiresInDays)));   
            }

            if (authenticateResult.Properties.Items.TryGetValue(OpenIdConnectSessionProperties.SessionState, out var sessionState))
            {
                claims.Add(new ClaimRecord("SessionState", sessionState));
            }   
        }

        return claims;
    }
}
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BudgetCast.Gateways.Bff.Models;
using BudgetCast.Gateways.Bff.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Gateways.Bff.Controllers;

[ApiController]
[Route("/bff/login")]
public class LoginController : ControllerBase
{
    private readonly IIdentityEndpointService _identityEndpointService;

    public LoginController(IIdentityEndpointService identityEndpointService)
    {
        _identityEndpointService = identityEndpointService;
    }
    
    [AllowAnonymous]
    [HttpPost("individual")]
    public async Task<IActionResult> LoginAsIndividualAsync([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var result = await _identityEndpointService.LoginAsync(dto, cancellationToken);

        if (!result.IsSuccess && result.StatusCode == 401)
        {
            return Unauthorized();
        }
        
        if (!string.IsNullOrWhiteSpace(result.AccessToken))
        {
            var claims = new[]
            {
                new Claim("uuid", Guid.NewGuid().ToString("N")),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authenticationTicket = new AuthenticationTicket(principal, AppConstants.DefaultAppAuthScheme);
            
            var expiration = new JwtSecurityTokenHandler()
                .ReadJwtToken(result.AccessToken).ValidTo;
            authenticationTicket.Properties.Items[AppConstants.ExpiresAtName] = expiration.ToString("o", CultureInfo.InvariantCulture);
            authenticationTicket.Properties.Items[AppConstants.AccessTokenName] = result.AccessToken;
            authenticationTicket.Properties.Items[AppConstants.RefreshTokenName] = "Not-passed";

            await HttpContext.SignInAsync(
                AppConstants.DefaultAppAuthScheme, 
                principal,
                authenticationTicket.Properties);
        }
            
        return NoContent();
    }
}
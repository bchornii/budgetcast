using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using BudgetCast.Gateways.Bff.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace BudgetCast.Gateways.Bff.Controllers;

[ApiController]
[Route("/bff/login")]
public class LoginController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    [AllowAnonymous]
    [HttpPost("individual")]
    public async Task<IActionResult> LoginAsIndividualAsync([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var identityClient = _httpClientFactory.CreateClient("identity");
        var responseMessage = await identityClient
            .PostAsJsonAsync("/api/signin/individual", dto, cancellationToken);
        
        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            var result = await responseMessage.Content
                .ReadFromJsonAsync<LoginVm>(cancellationToken: cancellationToken);

            if (!string.IsNullOrWhiteSpace(result?.AccessToken))
            {
                var claims = new[]
                {
                    new Claim("uuid", Guid.NewGuid().ToString("N")),
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var authenticationTicket =
                    new AuthenticationTicket(principal, CookieAuthenticationDefaults.AuthenticationScheme);
                
                var expiresName = "expires_at";
                var tokenName = OpenIdConnectParameterNames.AccessToken;
                var refreshTokenName = OpenIdConnectParameterNames.RefreshToken;
                var tokenJwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
                var expiration = tokenJwt.ValidTo;
                authenticationTicket.Properties.Items[$".Token.{expiresName}"] = expiration.ToString("o", CultureInfo.InvariantCulture);
                authenticationTicket.Properties.Items[$".Token.{tokenName}"] = result.AccessToken;
                authenticationTicket.Properties.Items[$".Token.{refreshTokenName}"] = "Not-passed";

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    principal,
                    authenticationTicket.Properties);
            }
            
            return NoContent();
        }

        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Unauthorized();
        }

        return new ObjectResult(null)
        {
            StatusCode = (int?)responseMessage.StatusCode,
        };
    }
}
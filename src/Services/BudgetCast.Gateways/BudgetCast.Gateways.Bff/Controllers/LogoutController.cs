using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Gateways.Bff.Controllers;

[ApiController]
[Route("bff/logout")]
public class LogoutController : ControllerBase
{
    [Authorize]
    [HttpPost("individual")]
    public async Task<IActionResult> LogoutAsIndividualAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }
}
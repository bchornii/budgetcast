using BudgetCast.Identity.Api.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignOutController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SignOutController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            user.ClearRefreshTokenInformation();
            await _userManager.UpdateAsync(user);
            
            return Ok();
        }
    }
}

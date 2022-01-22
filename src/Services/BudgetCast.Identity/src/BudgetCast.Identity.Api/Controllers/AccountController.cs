using BudgetCast.Identity.Api.ApiModels.Account;
using BudgetCast.Identity.Api.Database.Models;
using BudgetCast.Identity.Api.Infrastructure.AppSettings;
using BudgetCast.Identity.Api.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace BudgetCast.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly UiLinks _uiLinks;
        private readonly IStringLocalizer<TokenService> _localizer;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            UiLinks uiLinks, 
            IStringLocalizer<TokenService> localizer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _uiLinks = uiLinks;
            _localizer = localizer;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterVm model)
        {
            var user = model.GetUser();
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimsAsync(user, model.GetClaims());

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("confirmEmail", "Account",
                    new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
                await _emailService.ConfirmAccount(model.Email, callbackUrl!);

                return Ok();
            }

            return BadRequest(_localizer["User creation failed."]);
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileVm model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound(_localizer["User not found."]);
            }

            await RemoveClaimsAsync(user,
                ClaimTypes.GivenName, ClaimTypes.Surname);

            await _userManager.AddClaimsAsync(user, new[]
            {
                new Claim(ClaimTypes.GivenName, model.GivenName),
                new Claim(ClaimTypes.Surname, model.SurName)
            });

            await _signInManager.RefreshSignInAsync(user);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("email/confirm")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return Redirect(_uiLinks.Root);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Redirect(_uiLinks.Root);
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                user.IsActive = true;
                return Redirect(_uiLinks.Login);
            }

            return Redirect(_uiLinks.Root);
        }

        [AllowAnonymous]
        [HttpPost("password/forgot")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordVm model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return NotFound("User not found.");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{_uiLinks.ResetPassword}?userId={user.Id}&code={code}";
            await _emailService.ResetPassword(model.Email, callbackUrl);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("password/reset")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordVm model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(_localizer["User not found."]);
            }

            var result = await _userManager
                .ResetPasswordAsync(user, model.Code, model.Password);

            return result.Succeeded
                ? Ok()
                : BadRequest(_localizer["Reset password operation failed."]);
        }

        [AllowAnonymous]
        [HttpGet("isAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            return Ok(new
            {
                User.Identity!.IsAuthenticated,
                UserName = User.Claims.LastOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                GivenName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                SurName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
                FullName = User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.Fullname)?.Value,
                ImageUrl = User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.ImageUrl)?.Value,
            });
        }

        private async Task RemoveClaimsAsync(ApplicationUser user, params string[] claimType)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var claimsToRemove = claims.Where(c => claimType.Contains(c.Type));
            await _userManager.RemoveClaimsAsync(user, claimsToRemove);
        }
    }
}

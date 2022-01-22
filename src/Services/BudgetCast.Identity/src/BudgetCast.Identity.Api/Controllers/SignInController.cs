using BudgetCast.Identity.Api.ApiModels.Account;
using BudgetCast.Identity.Api.Database.Models;
using BudgetCast.Identity.Api.Infrastructure.AppSettings;
using BudgetCast.Identity.Api.Infrastructure.Extensions;
using BudgetCast.Identity.Api.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace BudgetCast.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ExternalIdentityProviders _externalIdentityProviders;
        private readonly ITokenService _tokenService;
        private readonly IStringLocalizer<SignInController> _localizer;

        public SignInController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ExternalIdentityProviders externalIdentityProviders,
            ITokenService tokenService, 
            IStringLocalizer<SignInController> localizer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _externalIdentityProviders = externalIdentityProviders;
            _tokenService = tokenService;
            _localizer = localizer;
        }

        [AllowAnonymous]
        [HttpGet("google")]
        public IActionResult SignInWithGoogle()
        {
            var googleProviderName = _externalIdentityProviders.Google.Name;
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties(
                googleProviderName, Url.Action(nameof(HandleExternalLogin)));
            return Challenge(authenticationProperties, googleProviderName);
        }

        [AllowAnonymous]
        [HttpGet("facebook")]
        public IActionResult SignInWithFacebook()
        {
            var facebookProviderName = _externalIdentityProviders.Facebook.Name;
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties(
                facebookProviderName, Url.Action(nameof(HandleExternalLogin)));
            return Challenge(authenticationProperties, facebookProviderName);
        }

        [AllowAnonymous]
        [HttpPost("individual")]
        public async Task<IActionResult> Login(
            [FromBody] LoginVm model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Unauthorized(_localizer["User not found."]);
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized(_localizer["You haven't confirmed account. Please check your email."]);
            }

            if (!user.IsActive)
            {
                return Unauthorized(_localizer["User is inactive."]);
            }


            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return Unauthorized(_localizer["Invalid credentials."]);
            }

            var tokenResponse = _tokenService.GetToken(user, GenerateIpAddress());
            user.RefreshToken = tokenResponse.RefreshToken;
            user.RefreshTokenExpiryTime = tokenResponse.RefreshTokenExpiryTime;
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                AccessToken = tokenResponse.Token,
            });
        }

        [AllowAnonymous]
        [HttpGet("handleExternalLogin")]
        public async Task<IActionResult> HandleExternalLogin()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            var result = await _signInManager
                .ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (!result.Succeeded)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(email);
                
                if (user == null)
                {
                    user = info.Principal.AsApplicationUser();
                    var createResult = await _userManager.CreateAsync(user);

                    if (!createResult.Succeeded)
                    {
                        throw new Exception(createResult.Errors.Select(e => e.Description).
                            Aggregate((errors, error) => $"{errors}, {error}"));
                    }
                }

                await _userManager.AddLoginAsync(user, info);

                var userClaims = info.Principal
                    .Claims.Append(new Claim(ClaimConstants.UserId, user.Id));
                await _userManager.AddClaimsAsync(user, userClaims);

                var tokenResponse = _tokenService.GetToken(user, GenerateIpAddress());
                user.RefreshToken = tokenResponse.RefreshToken;
                user.RefreshTokenExpiryTime = tokenResponse.RefreshTokenExpiryTime;
                await _userManager.UpdateAsync(user);

                await _signInManager.SignOutAsync();
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                await _signInManager.SignInFromAsync(tokenResponse.Token);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email is null)
                {
                    return Unauthorized(_localizer["External authentication failed."]);
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user is null)
                {
                    return Unauthorized(_localizer["External authentication failed."]);
                }

                var tokenResponse = _tokenService.GetToken(user, GenerateIpAddress());
                user.RefreshToken = tokenResponse.RefreshToken;
                user.RefreshTokenExpiryTime = tokenResponse.RefreshTokenExpiryTime;
                await _userManager.UpdateAsync(user);

                await _signInManager.SignOutAsync();
                await _signInManager.SignInFromAsync(tokenResponse.Token);
            }

            return Redirect(_externalIdentityProviders.UiRedirectUrl);
        }

        private string GenerateIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
            }
        }
    }
}

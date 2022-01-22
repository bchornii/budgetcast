using BudgetCast.Identity.Api.ApiModels.Account;
using BudgetCast.Identity.Api.ApiModels.Token;
using BudgetCast.Identity.Api.Database.Models;
using BudgetCast.Identity.Api.Infrastructure.AppSettings;
using BudgetCast.Identity.Api.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ExternalIdentityProviders _externalIdentityProviders;
        private readonly UiLinks _uiLinks;
        private readonly ITokenService _tokenService;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ExternalIdentityProviders externalIdentityProviders,
            UiLinks uiLinks, 
            ITokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _externalIdentityProviders = externalIdentityProviders;
            _uiLinks = uiLinks;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpGet("signInWithGoogle")]
        public IActionResult SignInWithGoogle()
        {
            var googleProviderName = _externalIdentityProviders.Google.Name;
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties(
                googleProviderName, Url.Action(nameof(HandleExternalLogin)));
            return Challenge(authenticationProperties, googleProviderName);
        }

        [AllowAnonymous]
        [HttpGet("signInWithFacebook")]
        public IActionResult SignInWithFacebook()
        {
            var facebookProviderName = _externalIdentityProviders.Facebook.Name;
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties(
                facebookProviderName, Url.Action(nameof(HandleExternalLogin)));
            return Challenge(authenticationProperties, facebookProviderName);
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
                var givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                var surName = info.Principal.FindFirstValue(ClaimTypes.Surname);
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        FirstName = givenName,
                        LastName = surName,
                        IsActive = true,
                    };
                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        throw new Exception(createResult.Errors.Select(e => e.Description).
                            Aggregate((errors, error) => $"{errors}, {error}"));
                    }
                }

                await _userManager.AddLoginAsync(user, info);

                var newUserClaims = info.Principal.Claims.Append(new Claim("userId", user.Id));
                await _userManager.AddClaimsAsync(user, newUserClaims);                
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                var tokenResponse = _tokenService.GetToken(user, GenerateIpAddress());
                user.RefreshToken = tokenResponse.RefreshToken;
                user.RefreshTokenExpiryTime = tokenResponse.RefreshTokenExpiryTime;
                await _userManager.UpdateAsync(user);

                HttpContext.Response.Cookies.Append("X-TOKEN", tokenResponse.Token, new CookieOptions
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = DateTimeOffset.MaxValue,
                });
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(email);
                if (user is null)
                {
                    return Unauthorized();
                }

                var tokenResponse = _tokenService.GetToken(user, GenerateIpAddress());
                user.RefreshToken = tokenResponse.RefreshToken;
                user.RefreshTokenExpiryTime = tokenResponse.RefreshTokenExpiryTime;
                // await _userManager.UpdateAsync(user);

                await _signInManager.SignOutAsync();
                HttpContext.Response.Cookies.Append("X-TOKEN", tokenResponse.Token, new CookieOptions
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.Strict,
                    Secure = true,
                    Expires = DateTimeOffset.Now.AddMinutes(5),
                });
            }

            return Redirect(_externalIdentityProviders.UiRedirectUrl);
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

            return BadRequest("User creation failed.");
        }

        [Authorize]
        [HttpPost("updateProfile")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileVm model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found.");
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
        [HttpGet("confirmEmail")]
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

            return result.Succeeded ? Redirect(_uiLinks.Login) : Redirect(_uiLinks.Root);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginVm model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest("You haven't confirmed account. Please check your email.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return BadRequest("Please check your credentials.");
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("forgotPassword")]
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
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordVm model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager
                .ResetPasswordAsync(user, model.Code, model.Password);

            return result.Succeeded
                ? Ok()
                : BadRequest("Reset password operation failed.");
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
                SurName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value
            });
        }

        [AllowAnonymous]
        [HttpGet("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestDto dto)
        {
            var token = await _tokenService.GetTokenAsync(dto, GenerateIpAddress());
            return Ok(token);
        }

        private async Task RemoveClaimsAsync(ApplicationUser user, params string[] claimType)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var claimsToRemove = claims.Where(c => claimType.Contains(c.Type));
            await _userManager.RemoveClaimsAsync(user, claimsToRemove);
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

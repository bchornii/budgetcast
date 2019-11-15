using BudgetCast.Dashboard.Api.AppSettings;
using BudgetCast.Dashboard.Api.Extensions;
using BudgetCast.Dashboard.Api.Services;
using BudgetCast.Dashboard.Api.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailService _emailService;
        private readonly ExternalIdentityProviders _externalIdentityProviders;

        public AccountController(
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            EmailService emailService,
            IOptions<ExternalIdentityProviders> options)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _externalIdentityProviders = options.Value;
        }

        [HttpGet("signInWithGoogle")]
        public IActionResult SignInWithGoogle()
        {
            var googleProviderName = _externalIdentityProviders.Google.Name;
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties(
                googleProviderName, Url.Action(nameof(HandleExternalLogin)));
            return Challenge(authenticationProperties, googleProviderName);
        }
        
        [HttpGet("handleExternalLogin")]
        public async Task<IActionResult> HandleExternalLogin()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            var result = await _signInManager
                .ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (!result.Succeeded)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var newUser = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var createResult = await _userManager.CreateAsync(newUser);
                if (!createResult.Succeeded)
                {
                    throw new Exception(createResult.Errors.Select(e => e.Description).
                        Aggregate((errors, error) => $"{errors}, {error}"));
                }

                await _userManager.AddLoginAsync(newUser, info);

                var newUserClaims = info.Principal.Claims.Append(new Claim("userId", newUser.Id));
                await _userManager.AddClaimsAsync(newUser, newUserClaims);
                await _signInManager.SignInAsync(newUser, isPersistent: false);                
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }

            return Redirect(_externalIdentityProviders.UiRedirectUrl);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterViewModel model)
        {
            var user = model.GetUser();
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {                
                await _userManager.AddClaimsAsync(user, model.GetClaims());

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("confirmEmail", "Account",
                    new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);                                
                await _emailService.SendEmailAsync(model.Email, callbackUrl);

                return Ok();
            }

            return BadRequest(result.GetErrorMessage());
        }

        [HttpPost("updateProfile")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found");
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

        [HttpGet("confirmEmail")]        
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if(string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return Redirect(_externalIdentityProviders.UiRedirectUrl);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return Redirect(_externalIdentityProviders.UiRedirectUrl);
            }

            await _userManager.ConfirmEmailAsync(user, code);

            return Redirect(_externalIdentityProviders.UiRedirectUrl);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if(user == null)
            {
                return Unauthorized();
            }

            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest("You haven't confirmed account. Please check your email.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }
            
            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("isAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            return Ok(new 
            {
                User.Identity.IsAuthenticated,
                UserName = User.Claims.LastOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                GivenName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                SurName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value
            });
        }        

        private async Task RemoveClaimsAsync(IdentityUser user, params string[] claimType)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var claimsToRemove = claims.Where(c => claimType.Contains(c.Type));
            await _userManager.RemoveClaimsAsync(user, claimsToRemove);
        }
    }
}
using BudgetCast.Identity.Api.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace BudgetCast.Identity.Api.Infrastructure.Extensions
{
    internal static class SignInManagerExtensions
    {
        public static Task SignInFromAsync(
            this SignInManager<ApplicationUser> signInManager, 
            string token)
        {
            signInManager.Context.Response.Cookies.Append("X-TOKEN", token, new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Expires = DateTimeOffset.Now.AddMinutes(5),
            });
            return Task.CompletedTask;
        }
    }
}

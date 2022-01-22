using BudgetCast.Identity.Api.Database.Models;
using System.Security.Claims;

namespace BudgetCast.Identity.Api.Infrastructure.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        public static ApplicationUser AsApplicationUser(this ClaimsPrincipal claimsPrincipal)
        {
            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            var givenName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName);
            var surName = claimsPrincipal.FindFirstValue(ClaimTypes.Surname);

            return new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = givenName,
                LastName = surName,
                IsActive = true,
            };
        }
    }
}

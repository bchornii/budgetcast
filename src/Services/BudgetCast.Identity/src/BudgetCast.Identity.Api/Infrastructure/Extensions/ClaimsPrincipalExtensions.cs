using BudgetCast.Identity.Api.Database.Models;
using System.Security.Claims;

namespace BudgetCast.Identity.Api.Infrastructure.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        private static readonly HashSet<string> IdClaims = new() 
        { 
            ClaimTypes.Email, ClaimTypes.GivenName, ClaimTypes.Surname
        };

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
                GivenName = givenName,
                LastName = surName,
                IsActive = true,
            };
        }

        public static IReadOnlyCollection<Claim> NonIdClaims(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.Where(c => !IdClaims.Contains(c.Type)).ToArray();
        }
    }
}

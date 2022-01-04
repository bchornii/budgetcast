using System.Security.Claims;

namespace BudgetCast.Common.Authentication
{
    public sealed class IdentityContext : IIdentityContext
    {
        /// <summary>
        /// Represent non-authenticated user identity context.
        /// </summary>
        public static readonly IdentityContext NonAuthenticated = new();

        public IdentityContext()
        {
            UserIdentity = new ClaimsPrincipal(new ClaimsIdentity());
        }

        public ClaimsPrincipal UserIdentity { get; set; }

        public string UserId => UserIdentity.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}

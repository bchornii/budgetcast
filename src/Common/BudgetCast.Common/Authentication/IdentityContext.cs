using System.Security.Claims;

namespace BudgetCast.Common.Authentication
{
    public sealed class IdentityContext : IIdentityContext
    {
        /// <summary>
        /// Represent non-authenticated user identity context.
        /// </summary>
        public static readonly IdentityContext NonAuthenticated = new(default!, 0);

        public string UserId { get; }

        public long TenantId { get; }

        public IdentityContext(string userId, long tenantId)
        {
            UserId = userId;
            TenantId = tenantId;
        }
    }
}

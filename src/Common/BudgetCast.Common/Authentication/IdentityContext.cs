namespace BudgetCast.Common.Authentication
{
    public sealed class IdentityContext : IIdentityContext
    {
        /// <summary>
        /// Represent non-authenticated user identity context.
        /// </summary>
        public static readonly IdentityContext NonAuthenticated = new(default!);

        public string UserId { get; }

        public long? TenantId { get; private set; }

        public IdentityContext(string userId)
        {
            UserId = userId;
        }

        public void SetCurrentTenant(long tenantId)
        {
            TenantId = tenantId;
        }
    }
}

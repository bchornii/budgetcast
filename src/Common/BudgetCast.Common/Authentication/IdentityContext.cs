namespace BudgetCast.Common.Authentication
{
    public sealed class IdentityContext : IIdentityContext
    {
        /// <summary>
        /// Represent non-authenticated user identity context.
        /// </summary>
        public static readonly IdentityContext NonAuthenticated = new(default!);

        /// <summary>
        /// Represent identity context which wasn't constructed due to some runtime limitations.
        /// </summary>
        public static readonly IdentityContext NotConstructed = new(default!);

        public string UserId { get; private set; }

        public long? TenantId { get; private set; }

        public IdentityContext(string userId)
        {
            UserId = userId;
        }

        public void SetCurrentTenant(long tenantId)
        {
            TenantId = tenantId;
        }

        public void SetUserId(string userId)
            => UserId = UserId ?? userId;
    }
}

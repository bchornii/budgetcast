namespace BudgetCast.Common.Authentication
{
    public sealed class IdentityContext : IIdentityContext
    {
        public string UserId { get; private set; }

        public long? TenantId { get; private set; }

        public bool HasAssociatedTenant => TenantId.HasValue;

        public bool HasAssociatedUser => !string.IsNullOrWhiteSpace(UserId);

        public IdentityContext(string userId)
        {
            UserId = userId;
        }

        public void SetCurrentTenant(long tenantId)
        {
            TenantId = tenantId;
        }

        public void SetUserId(string userId)
        {
            UserId = UserId ?? userId;
        }

        public static IdentityContext GetNewEmpty()
            => new(default!);
    }
}

namespace BudgetCast.Common.Authentication
{
    public interface IIdentityContext
    {
        /// <summary>
        /// User identifier. Globally unique value across the system.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Tenant identifier. May be null initially, but for the most
        /// operation is required.
        /// </summary>
        long? TenantId { get; }

        /// <summary>
        /// Sets tenant identifier.
        /// </summary>
        /// <param name="tenantId"></param>
        void SetCurrentTenant(long tenantId);
    }
}

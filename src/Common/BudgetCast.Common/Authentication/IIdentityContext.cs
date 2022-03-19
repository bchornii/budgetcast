namespace BudgetCast.Common.Authentication
{
    public interface IIdentityContext
    {
        /// <summary>
        /// Allows to verify if <seealso cref="IIdentityContext"/> has been initialized.
        /// </summary>
        bool HasAssociatedTenant { get; }

        /// <summary>
        /// Allows to verify if <seealso cref="IIdentityContext"/> has been authenticated.
        /// </summary>
        bool HasAssociatedUser { get; }

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

        /// <summary>
        /// Sets user identifier but only in case it wasn't set during initial 
        /// <see cref="IdentityContext"/> construction.
        /// </summary>
        /// <param name="userId"></param>
        void SetUserId(string userId);
    }
}

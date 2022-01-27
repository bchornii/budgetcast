namespace BudgetCast.Common.Web.Middleware
{
    public interface ITenantService
    {
        long TenantId { get; }

        void SetCurrentTenant(long tenantId);

        long GetCurrentTenant();
    }
}

namespace BudgetCast.Common.Web.Middleware
{
    public class TenantService : ITenantService
    {
        public long TenantId { get; private set; }

        public void SetCurrentTenant(long tenantId)
        {
            TenantId = tenantId;
        }

        public long GetCurrentTenant()
            => TenantId;
    }
}

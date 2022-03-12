namespace BudgetCast.Common.Domain;

public class AggregateRoot : Entity, IMustHaveTenant
{
    public long TenantId { get; set; }

    public void SetTenant(long tenantId)
    {
        TenantId = tenantId;
    }
}
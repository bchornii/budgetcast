namespace BudgetCast.Common.Domain;

public interface IMustHaveTenant
{
    public long TenantId { get; set; }
}
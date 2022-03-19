using BudgetCast.Common.Messaging.Abstractions.Events;

namespace BudgetCast.Expenses.Messaging;

public class ExpensesRemovedEvent : IntegrationEvent
{
    public long TenantId { get; }
    
    public long ExpenseId { get; }
    
    public decimal Total { get; }
    
    public string AddedBy { get; }
    
    public DateTime AddedAt { get; }
    
    public string CampaignName { get; }

    public ExpensesRemovedEvent(
        long tenantId, 
        long expenseId, 
        decimal total, 
        string addedBy, 
        DateTime addedAt, 
        string campaignName)
    {
        TenantId = tenantId;
        ExpenseId = expenseId;
        Total = total;
        AddedBy = addedBy;
        AddedAt = addedAt;
        CampaignName = campaignName;

        Id = $"{TenantId}-{ExpenseId}";
    }
}
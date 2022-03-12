namespace BudgetCast.Common.Domain;

public interface ISoftDelete
{
    DateTime? DeletedOn { get; set; }

    string? DeletedBy { get; set; }
}
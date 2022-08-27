using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Common.Domain;

public interface IBusinessRule
{
    IBusinessRule And();
    
    Result Validate();
    
    Task<Result> ValidateAsync(CancellationToken cancellationToken);
}

public interface IBusinessRule<in TData> : IBusinessRule
{
    Result ValidateAgainst(TData data);
    
    Task<Result> ValidateAgainstAsync(TData data, CancellationToken cancellationToken);
}
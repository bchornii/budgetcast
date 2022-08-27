using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Common.Domain;

public abstract class AbstractBusinessRule : IBusinessRule
{
    public IBusinessRule And() 
        => this;

    public virtual Result Validate() 
        => Success.Empty;
    
    public virtual Task<Result> ValidateAsync(CancellationToken cancellationToken) 
        => Task.FromResult<Result>(Success.Empty);
}

public abstract class AbstractBusinessRule<TData> : AbstractBusinessRule, IBusinessRule<TData>
{
    public virtual Result ValidateAgainst(TData data)
        => Success.Empty;

    public virtual Task<Result> ValidateAgainstAsync(
        TData data, 
        CancellationToken cancellationToken)
        => Task.FromResult<Result>(Success.Empty);
}
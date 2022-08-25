namespace BudgetCast.Common.Application.Behavior.Authorization;

public abstract class AbstractAuthorizer<TRequest> : IAuthorizer<TRequest>
{
    private readonly HashSet<IAuthorizationRequirement> _requirements = new();

    public IEnumerable<IAuthorizationRequirement> Requirements => _requirements;

    public abstract void BuildPolicy(TRequest instance);

    protected void UseRequirement(IAuthorizationRequirement requirement)
    {
        if (requirement == null)
        {
            return;
        }
        
        _requirements.Add(requirement);
    }
}
namespace BudgetCast.Common.Application.Behavior.Authorization;

public interface IAuthorizer<T>
{
    /// <summary>
    /// Returns collection of authorization requirements.
    /// </summary>
    IEnumerable<IAuthorizationRequirement> Requirements { get; }
    
    /// <summary>
    /// Builds authorization policy based on set of <see cref="Requirements"/>.
    /// </summary>
    /// <param name="instance"></param>
    void BuildPolicy(T instance);
}
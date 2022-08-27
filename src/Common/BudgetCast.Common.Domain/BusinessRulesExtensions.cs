using System.Runtime.CompilerServices;
using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Common.Domain;

public static class BusinessRulesExtensions
{
    public static IEnumerable<IBusinessRule> Locate(
        this IBusinessRuleRegistry registry, 
        params Type[] rulesTypes) 
        => rulesTypes.Select(registry.Locate);

    public static async IAsyncEnumerable<Result> ValidateAsync(
        this IEnumerable<IBusinessRule> rules, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var rule in rules)
        {
            var result = await rule.ValidateAsync(cancellationToken);
            yield return result;
        }
    }
}
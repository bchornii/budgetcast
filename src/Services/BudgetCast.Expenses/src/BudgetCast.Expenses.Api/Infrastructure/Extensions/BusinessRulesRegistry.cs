using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Api.Infrastructure.Extensions;

public class BusinessRulesRegistry : IBusinessRuleRegistry
{
    private readonly IServiceProvider _serviceProvider;

    public BusinessRulesRegistry(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IBusinessRule Locate(Type ruleType)
    {
        var rule = _serviceProvider.GetRequiredService(ruleType);
        return (rule as IBusinessRule)!;
    }

    public IBusinessRule Locate<TRule>()
        where TRule : IBusinessRule
    {
        var rule = _serviceProvider.GetRequiredService<TRule>();
        return rule;
    }

    public IBusinessRule<TData> Locate<TRule, TData>() 
        where TRule : IBusinessRule<TData>
    {
        var rule = _serviceProvider.GetRequiredService<TRule>();
        return rule;
    }
}
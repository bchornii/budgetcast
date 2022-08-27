namespace BudgetCast.Common.Domain;

public interface IBusinessRuleRegistry
{
    IBusinessRule Locate(Type ruleType);
    
    IBusinessRule Locate<TRule>()
        where TRule : IBusinessRule;
    
    IBusinessRule<TData> Locate<TRule, TData>() 
        where TRule : IBusinessRule<TData>;
}
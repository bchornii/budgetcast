using BudgetCast.Common.Application.Behavior.Authorization;
using BudgetCast.Common.Authentication;
using BudgetCast.Expenses.Commands.Requirements;

namespace BudgetCast.Expenses.Commands.Expenses;

public class AddExpenseAuthorizer : AbstractAuthorizer<AddExpenseCommand>
{
    private readonly IIdentityContext _identityContext;

    public AddExpenseAuthorizer(IIdentityContext identityContext) 
        => _identityContext = identityContext;

    public override void BuildPolicy(AddExpenseCommand instance)
    {
        UseRequirement(new MustHaveTenantIdRequirement(_identityContext.TenantId!.Value));
    }
}
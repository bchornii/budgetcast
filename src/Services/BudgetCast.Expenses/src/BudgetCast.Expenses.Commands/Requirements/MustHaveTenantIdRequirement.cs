using BudgetCast.Common.Application.Behavior.Authorization;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Expenses.Commands.Requirements;

public record MustHaveTenantIdRequirement(long TenantId) : IAuthorizationRequirement;

public class MustHaveTenantIdRequirementHandler : IAuthorizationHandler<MustHaveTenantIdRequirement>
{
    private readonly IIdentityContext _identityContext;

    public MustHaveTenantIdRequirementHandler(IIdentityContext identityContext) 
        => _identityContext = identityContext;

    public async Task<Result<AuthorizationResult>> Handle(MustHaveTenantIdRequirement request, CancellationToken cancellationToken)
    {
        var tenantId = _identityContext.TenantId;

        if (tenantId == request.TenantId)
        {
            return AuthorizationResult.Succeed();
        }

        return AuthorizationResult.Fail("You don't have required tenant id");
    }
}
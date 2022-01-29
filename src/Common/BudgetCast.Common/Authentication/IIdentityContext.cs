using System.Security.Claims;

namespace BudgetCast.Common.Authentication
{
    public interface IIdentityContext
    {
        string UserId { get; }

        long TenantId { get; }
    }
}

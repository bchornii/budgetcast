using System.Security.Claims;

namespace BudgetCast.Gateways.Bff.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUuid(this ClaimsPrincipal? principal)
        => principal?.Claims.FirstOrDefault(c => c.Type == "uuid")?.Value;
}
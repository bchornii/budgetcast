using BudgetCast.Gateways.Bff.Models;
using BudgetCast.Gateways.Bff.Services;

namespace BudgetCast.Gateways.Bff.Extensions;

public static class Extensions
{
    public static async Task<string> GetManagedAccessToken(this HttpContext context, TokenType tokenType)
    {
        if (tokenType == TokenType.User)
        {
            return await context.GetUserAccessTokenAsync();
        }
        
        return await context.GetUserAccessTokenAsync();
    }
    
    public static async Task<string> GetUserAccessTokenAsync(this HttpContext httpContext, CancellationToken cancellationToken = default)
    {
        var service = httpContext.RequestServices.GetRequiredService<IUserAccessTokenManagementService>();

        return await service.GetUserAccessTokenAsync(httpContext.User, cancellationToken);
    }
}
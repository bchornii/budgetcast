using BudgetCast.Common.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Operations;
using BudgetCast.Common.Web.Messaging;

namespace BudgetCast.Common.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityContext(this IServiceCollection services)
    {
        services.AddScoped<IIdentityContext, IdentityContext>(provider =>
        {                
            var httpCtxAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpCtxAccessor.HttpContext;

            if (httpContext is null)
            {
                return IdentityContext.GetNewEmpty();
            }

            var principal = httpCtxAccessor.HttpContext.User;
            if (!principal.IsAnyIdentityAuthenticated())
            {
                return IdentityContext.GetNewEmpty();
            }

            var userId = principal.Claims
                .First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var identityContext = new IdentityContext(userId: userId);

            return identityContext;
        });

        return services;
    }
    
    /// <summary>
    /// Adds OperationContext instance to DI, so interested service can retrieve it and
    /// get the information about a distributed operation
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddOperationContext(this IServiceCollection services)
    {
        services.AddScoped(provider =>
        {
            var http = provider.GetRequiredService<IHttpContextAccessor>();

            if (http.HttpContext.Request.Headers.ContainsKey(OperationContext.HeaderName))
            {
                var operationContextHeader = http.HttpContext.Request.Headers[OperationContext.HeaderName];
                return OperationContext.Unpack(operationContextHeader);
            }

            // If the value was not present in the headers - assume this is the start
            return OperationContext.New();
        });

        return services;
    }

    /// <summary>
    /// Registers messaging pre- and post- sending/processing steps to supplement it
    /// with a contextual information provided by the application
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMessagingExtensions(this IServiceCollection services)
    {
        services.AddScoped<IMessagePreSendingStep, AddTenantToMessageMetadataStep>();
        services.AddScoped<IMessagePreSendingStep, AddUserToMessageMetadataStep>();
        
        services.AddScoped<IMessagePreProcessingStep, ExtractTenantFromMessageMetadataStep>();
        services.AddScoped<IMessagePreProcessingStep, ExtractUserFromMessageMetadataStep>();
        return services;
    }

    public static bool IsAnyIdentityAuthenticated(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Identities.Any(i => i.IsAuthenticated);
}
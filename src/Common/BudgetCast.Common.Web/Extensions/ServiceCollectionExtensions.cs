using BudgetCast.Common.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Operations;
using BudgetCast.Common.Web.Contextual;
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
            var http = provider.GetService<IHttpContextAccessor>();
            var httpContext = http?.HttpContext;

            // If it's downstream HTTP call context
            if (httpContext is not null && httpContext.Request.Headers.ContainsKey(OperationContext.MetaName))
            {
                var operationContextHeader = httpContext.Request.Headers[OperationContext.MetaName];
                return OperationContext.Unpack(operationContextHeader);
            }
            
            // If it's ongoing call and operation context has already been initialized (Http-based workload)
            if (httpContext is not null && httpContext.Items.ContainsKey(OperationContext.MetaName))
            {
                return (OperationContext)httpContext.Items[OperationContext.MetaName];
            }
            
            // If it's ongoing call and operation context has already been initialized (Non Http-based workload)
            var workloadContext = provider.GetRequiredService<WorkloadContext>();
            if (workloadContext.Contains(OperationContext.MetaName))
            {
                return (OperationContext)workloadContext.GetItem(OperationContext.MetaName);
            }

            // If previous checks failed assume it's a new operation context
            return OperationContext.New();
        });

        services.AddWorkloadContext();

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
        services.AddScoped<IMessagePreSendingStep, AddOrUpdateOperationContextStep>();
        
        services.AddScoped<IMessagePreProcessingStep, ExtractTenantFromMessageMetadataStep>();
        services.AddScoped<IMessagePreProcessingStep, ExtractUserFromMessageMetadataStep>();
        services.AddScoped<IMessagePreProcessingStep, ExtractOperationContextFromMessageMetadataStep>();
        return services;
    }

    public static bool IsAnyIdentityAuthenticated(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Identities.Any(i => i.IsAuthenticated);
    
    /// <summary>
    /// Registers <see cref="WorkloadContext"/> as a <see cref="ServiceLifetime.Scoped"/> service.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private static IServiceCollection AddWorkloadContext(this IServiceCollection services)
    {
        services.AddScoped<WorkloadContext>();
        return services;
    }
}
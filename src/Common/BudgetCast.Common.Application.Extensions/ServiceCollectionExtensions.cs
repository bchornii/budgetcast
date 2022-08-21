using System.Reflection;
using BudgetCast.Common.Application.Behavior.Authorization;
using BudgetCast.Common.Application.Behavior.Idempotency;
using BudgetCast.Common.Application.Behavior.Logging;
using BudgetCast.Common.Application.Behavior.Validation;
using BudgetCast.Common.Operations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BudgetCast.Common.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        Type? operationRegistryType = null,
        params Assembly[] commandAndQueryAssemblies)
    {
        services.AddMediatR(commandAndQueryAssemblies);

        // Register MediatR pipelines for logging, validation and idempotency
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddTransient(service =>
        {
            var options = service.GetRequiredService<IOptionsMonitor<LoggingOptions>>();
            var currentValue = options.CurrentValue;
            return new LoggingBehaviorSetting(
                enableRequestPayloadTrace: currentValue.IncludeRequestBody,
                enableResponsePayloadTrace: currentValue.IncludePayload);
        });
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(IdempotentBehavior<,>));

        foreach (var assembly in commandAndQueryAssemblies)
        {
            services.AddAuthorizersFromAssembly(assembly);
        }
        
        if (operationRegistryType is null)
        {
            services.AddScoped<IOperationsRegistry, NoStorageOperationsRegistry>();
        }
        else if (operationRegistryType == typeof(InMemoryOperationRegistry))
        {
            services.AddScoped<IOperationsRegistry, InMemoryOperationRegistry>();
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddScoped(typeof(IOperationsRegistry), operationRegistryType);
        }

        // Register Fluent Validators from the same assembly where Commands/Queries
        foreach (var assembly in commandAndQueryAssemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }

        return services;
    }
    
    public static void AddAuthorizersFromAssembly(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var authorizerType = typeof(IAuthorizer<>);
        assembly.GetTypesAssignableTo(authorizerType).ForEach((type) =>
        {
            foreach (var implementedInterface in type.ImplementedInterfaces)
            {
                switch (lifetime)
                {
                    case ServiceLifetime.Scoped:
                        services.AddScoped(implementedInterface, type);
                        break;
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(implementedInterface, type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(implementedInterface, type);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
                }
            }
        });
    }
    
    private static List<TypeInfo> GetTypesAssignableTo(this Assembly assembly, Type compareType)
    {
        var typeInfoList = assembly.DefinedTypes
            .Where(x => 
                x.IsClass && 
                !x.IsAbstract && 
                x != compareType && 
                x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == compareType))
            .ToList();

        return typeInfoList;
    }
}
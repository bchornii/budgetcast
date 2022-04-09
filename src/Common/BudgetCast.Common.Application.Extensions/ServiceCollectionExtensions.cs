using System.Reflection;
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
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(IdempotentBehavior<,>));

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
}
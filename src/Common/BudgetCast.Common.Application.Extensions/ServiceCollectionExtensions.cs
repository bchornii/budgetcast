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
            params Assembly[] assemblies)
        {
            services.AddMediatR(assemblies);

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

            services.AddScoped<IOperationsRegistry, NoStorageOperationsRegistry>();

            // Register Fluent Validators from the same assembly where Commands/Queries
            foreach (var assembly in assemblies)
            {
                services.AddValidatorsFromAssembly(assembly);
            }

            return services;
        }
    }
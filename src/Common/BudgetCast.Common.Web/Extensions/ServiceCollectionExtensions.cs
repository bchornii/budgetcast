using BudgetCast.Common.Application.Behavior.Idempotency;
using BudgetCast.Common.Application.Behavior.Logging;
using BudgetCast.Common.Application.Behavior.Validation;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BudgetCast.Common.Web.Extensions
{
    public record LoggingOptions(bool IncludeRequestBody, bool IncludePayload);

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services, 
            params Type[] handlerAssemblyMarkerTypes)
        {
            services.AddMediatR(handlerAssemblyMarkerTypes);

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

            // Register Fluent Validators from the same assembly where Commands/Queries
            services.AddValidatorsFromAssemblyContaining(handlerAssemblyMarkerTypes[0]);

            return services;
        }
    }
}

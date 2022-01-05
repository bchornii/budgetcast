using BudgetCast.Common.Application.Behavior.Idempotency;
using BudgetCast.Common.Application.Behavior.Logging;
using BudgetCast.Common.Application.Behavior.Validation;
using BudgetCast.Common.Authentication;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Security.Claims;

namespace BudgetCast.Common.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services, 
            Assembly assembly)
        {
            services.AddMediatR(assembly);

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
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }

        public static IServiceCollection AddFakeIdentityContext(
            this IServiceCollection services)
        {
            services.AddScoped<IIdentityContext>(factory =>
            {
                return new IdentityContext
                {
                    UserIdentity = new ClaimsPrincipal(new ClaimsIdentity[]
                    {
                        new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, "bchornii")
                        })
                    })
                };
            });

            return services;
        }
    }
}

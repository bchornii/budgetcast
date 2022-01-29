using BudgetCast.Common.Application.Behavior.Idempotency;
using BudgetCast.Common.Application.Behavior.Logging;
using BudgetCast.Common.Application.Behavior.Validation;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Web.Middleware;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
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

        public static IServiceCollection AddIdentityContext(this IServiceCollection services)
        {
            services.AddScoped<IIdentityContext, IdentityContext>(provider =>
            {
                var http = provider.GetRequiredService<IHttpContextAccessor>();
                var principal = http.HttpContext.User;

                if (!principal.IsAnyIdentityAuthenticated())
                {
                    return IdentityContext.NonAuthenticated;
                }

                var userId = principal.Claims
                    .First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                var identityContext = new IdentityContext(userId: userId);

                return identityContext;
            });

            return services;
        }

        private static bool IsAnyIdentityAuthenticated(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.Identities.Any(i => i.IsAuthenticated);
    }
}

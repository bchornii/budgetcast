using BudgetCast.Common.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace BudgetCast.Common.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds current tenant middleware.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="pathsToExlude"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCurrentTenant(this IApplicationBuilder app, string[]? pathsToExlude = null)
        {
            var configuration = new CurrentTenantMiddlewareConfiguration();
            if (pathsToExlude is not null)
            {
                configuration.AddRange(pathsToExlude);
            }
            return app.UseMiddleware<CurrentTenantMiddleware>(configuration);
        }

        /// <summary>
        /// Adds exception handler middleware with custom handle logic into the pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="isDevelopment"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app, bool isDevelopment)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = async context =>
                {
                    var exceptionId = Guid.NewGuid();

                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var connection = context.Features.Get<IHttpConnectionFeature>();

                    await WriteGeneralExceptionResponseAsync(context, isDevelopment, exceptionFeature, exceptionId, connection);
                },
            });

            return app;
        }

        private static Task WriteGeneralExceptionResponseAsync(
            HttpContext context,
            bool isDevelopment,
            IExceptionHandlerFeature exceptionFeature,
            Guid exceptionId,
            IHttpConnectionFeature connection)
        {
            var routeData = context.GetRouteData() ?? new RouteData();
            var actionContext = new ActionContext(context, routeData, new ActionDescriptor());
            var result = new ObjectResult(new ProblemDetails
            {
                Detail = isDevelopment
                    ? exceptionFeature.Error.StackTrace
                    : "Check logs with the provided traceId",
                Title = "Error processing the request",
                Status = (int)HttpStatusCode.InternalServerError,
                Extensions =
                {
                    { "traceId", exceptionId.ToString() },
                    { "connectionId", connection.ConnectionId },
                },
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Instance = "ApiExceptionHandling",
            })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };

            var executor = context.RequestServices
                .GetRequiredService<IActionResultExecutor<ObjectResult>>();

            return executor.ExecuteAsync(actionContext, result);
        }
    }
}

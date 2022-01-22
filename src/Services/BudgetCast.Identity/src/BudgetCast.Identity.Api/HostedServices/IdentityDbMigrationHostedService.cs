using BudgetCast.Identity.Api.Database;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace BudgetCast.Identity.Api.HostedServices
{
    public class IdentityDbMigrationHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public IdentityDbMigrationHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<AppIdentityContext>>();
            var context = services.GetService<AppIdentityContext>()!;

            try
            {
                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(AppIdentityContext).Name);

                var retries = 10;
                var retry = Policy.Handle<SqlException>()
                    .WaitAndRetryAsync(
                        retryCount: retries,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (exception, timeSpan, retryNumber, ctx) =>
                        {
                            logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", nameof(AppIdentityContext),
                                exception.GetType().Name, exception.Message, retryNumber, retries);
                        });

                //if the sql server container is not created on run docker compose this
                //migration can't fail for network related exception. The retry options for DbContext only 
                //apply to transient exceptions
                // Note that this is NOT applied when running some orchestrators (let the orchestrator to recreate the failing service)
                await retry.ExecuteAsync(() => context.Database.MigrateAsync(cancellationToken));

                logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(AppIdentityContext).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(AppIdentityContext).Name);
            }

            await context.Database.MigrateAsync(cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}

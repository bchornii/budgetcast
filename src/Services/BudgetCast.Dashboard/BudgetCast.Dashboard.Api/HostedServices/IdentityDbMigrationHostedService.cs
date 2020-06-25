using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace BudgetCast.Dashboard.Api.HostedServices
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
            var logger = services.GetRequiredService<ILogger<IdentityDbContext>>();
            var context = services.GetService<IdentityDbContext>();

            try
            {
                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(IdentityDbContext).Name);

                var retries = 10;
                var retry = Policy.Handle<SqlException>()
                    .WaitAndRetryAsync(
                        retryCount: retries,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (exception, timeSpan, retryNumber, ctx) =>
                        {
                            logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", nameof(IdentityDbContext),
                                exception.GetType().Name, exception.Message, retryNumber, retries);
                        });

                //if the sql server container is not created on run docker compose this
                //migration can't fail for network related exception. The retry options for DbContext only 
                //apply to transient exceptions
                // Note that this is NOT applied when running some orchestrators (let the orchestrator to recreate the failing service)
                await retry.ExecuteAsync(() => context.Database.MigrateAsync(cancellationToken));

                logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(IdentityDbContext).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(IdentityDbContext).Name);
            }

            await context.Database.MigrateAsync(cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}

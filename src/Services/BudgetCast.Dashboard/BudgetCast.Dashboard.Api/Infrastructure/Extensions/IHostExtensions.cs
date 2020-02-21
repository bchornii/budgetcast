using System;
using BudgetCast.Dashboard.Api.Infrastructure.AppSettings;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace BudgetCast.Dashboard.Api.Infrastructure.Extensions
{
    public static class IHostExtensions
    {
        public static IHost CreateBlobContainers(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var config = scope.ServiceProvider.GetService<IConfiguration>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CloudBlobClient>>();

                try
                {
                    logger.LogInformation("Creating az blob containers.");

                    var azBlobStorageSettings = config.GetSection(
                        "AzBlobStorage").Get<AzBlobStorageSettings>();

                    var azBlobStorageContainersSettings = config.GetSection(
                        "AzBlobStorageContainers").Get<AzBlobStorageContainersSettings>();

                    var retries = 10;
                    var retry = GetRetryPolicy(logger, retries);
                    var client = CloudStorageAccount
                        .Parse(azBlobStorageSettings.ConnectionString)
                        .CreateCloudBlobClient();

                    foreach (var containerName in azBlobStorageContainersSettings.Containers)
                    {
                        var container = client.GetContainerReference(containerName);
                        retry.Execute(() => container.CreateIfNotExists());
                    }

                }
                catch (Exception e)
                {
                    logger.LogInformation($"Creating az blob containers failed with exception message {e.Message}.");
                }
            }

            return webHost;
        }

        public static IHost MigrateDbContext<TContext>(this IHost webHost, 
            Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                    var retries = 10;
                    var retry = GetRetryPolicy(logger, retries);

                    //if the sql server container is not created on run docker compose this
                    //migration can't fail for network related exception. The retry options for DbContext only 
                    //apply to transient exceptions
                    // Note that this is NOT applied when running some orchestrators (let the orchestrator to recreate the failing service)
                    retry.Execute(() => InvokeSeeder(seeder, context, services));

                    logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context " +
                        "{DbContextName}", typeof(TContext).Name);                    
                }
            }

            return webHost;
        }

        private static void InvokeSeeder<TContext>(
            Action<TContext, IServiceProvider> seeder, 
            TContext context, IServiceProvider services)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }

        private static RetryPolicy GetRetryPolicy(ILogger logger, int retries)
        {
            return Policy.Handle<Exception>()
                .WaitAndRetry(
                    retryCount: retries,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryNumber, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message " +
                                                     "{Message} detected on attempt {retry} of {retries}",
                            exception.GetType().Name, exception.Message, retryNumber, retries);
                    });
        }
    }
}

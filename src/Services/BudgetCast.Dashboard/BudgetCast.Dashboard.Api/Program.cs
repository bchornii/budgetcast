using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace BudgetCast.Dashboard.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host");
                var host = CreateHostBuilder(args).Build();

                Log.Information("Starting web host");
                host.Run();              
                
                return 0;
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly");
                return 1;                
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((context, config) =>
                    {
                        if (context.HostingEnvironment.IsProduction())
                        {
                            var builtConfig = config.Build();

                            var azureServiceTokenProvider = new AzureServiceTokenProvider();
                            var keyVaultClient = new KeyVaultClient(
                                new KeyVaultClient.AuthenticationCallback(
                                    azureServiceTokenProvider.KeyVaultTokenCallback));

                            config.AddAzureKeyVault(
                                $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
                                keyVaultClient, new DefaultKeyVaultSecretManager());
                        }
                    })
                    .UseStartup<Startup>();

                });

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var env = Environment.GetEnvironmentVariable(
                "ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(
                    path: "appsettings.json",
                    optional: false,
                    reloadOnChange: true)
                .AddJsonFile(
                    path: $"appsettings.{env}.json",
                    optional: false,
                    reloadOnChange: true)                
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}

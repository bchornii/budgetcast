using BudgetCast.Common.Domain;
using BudgetCast.Expenses.Data;
using BudgetCast.Expenses.Data.Campaigns;
using BudgetCast.Expenses.Data.Expenses;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using BudgetCast.Expenses.Queries.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace BudgetCast.Expenses.Api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Budget Cast Expenses API", Version = "v1" });
            });

            return services;
        }

        public static IServiceCollection AddCustomControllers(
            this IServiceCollection services)
        {
            services.AddControllers();

            return services;
        }

        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder
                .AddSqlServer(
                    configuration["ExpensesDb:ConnectionString"],
                    name: "Expenses-SqlDb-Check",
                    tags: new string[] { "expensesdb" });

            return services;
        }

        public static IServiceCollection AddData(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddDbContext<ExpensesDbContext>(options =>
            {
                options.UseSqlServer(configuration["ExpensesDb:ConnectionString"],
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });

                if (env.IsDevelopment())
                {
                    options.LogTo(Console.WriteLine, LogLevel.Information, DbContextLoggerOptions.LocalTime);
                    options.EnableSensitiveDataLogging();
                }
            });

            services.AddScoped<IUnitOfWork>(services =>
                    services.GetRequiredService<ExpensesDbContext>());

            services.AddScoped<ISqlConnectionFactory>(services => 
                new SqlConnectionFactory(configuration["ExpensesDb:ConnectionString"]));

            return services;
        }

        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddScoped<ICampaignRepository, CampaignRepository>();
            services.AddScoped<IExpensesRepository, ExpensesRepository>();
            return services;
        }
    }
}

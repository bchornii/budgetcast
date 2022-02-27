using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.AzServiceBus.Common;
using BudgetCast.Common.Messaging.AzServiceBus.Common.PreHandling;
using BudgetCast.Common.Messaging.AzServiceBus.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.AzServiceBus.Extensions;

// TODO: 1) create az service bus resource
// TODO: 2) create budgetcast_events_topic topic
// TODO: 3) add TestEvent correlation filter based on Subject message property
// TODO: 4) send message into topic - debug sending + receiving

public static class HostExtensions
{
    public static IHostBuilder UseAzServiceBus(
        this IHostBuilder hostBuilder, 
        Action<IServiceCollection> registerHandlers,
        Action<IEventProcessor> subscribeToEvents,
        Action<AzServiceBusConfiguration>? options = null)
    {
        hostBuilder.ConfigureServices((ctx, services) =>
        {
            AzServiceBusConfiguration? config = null;
            if (options is not null)
            {
                config = new AzServiceBusConfiguration();
                options(config);   
            }

            services.AddSingleton(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                return new EventBusClient(
                    connectionString: config?.AzureServiceBusConnectionString ?? 
                                      configuration["ServiceBusSettings:EventBusConnection"]);
            });
            
            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddScoped<IMessagePreProcessor, MessagePreProcessor>();
            
            services.AddSingleton<IEventSubscriptionManager, EventSubscriptionManager>();
            services.AddSingleton<IEventProcessor, EventProcessor>(provider =>
            {
                var subscriptionManager = provider.GetRequiredService<IEventSubscriptionManager>();
                var logger = provider.GetRequiredService<ILogger<EventProcessor>>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                var processingPipeline = provider.GetRequiredService<IMessageHandlingPipeline>();
                var eventBusClient = provider.GetRequiredService<EventBusClient>();
                var eventProcessor = new EventProcessor(
                    subscriptionManager: subscriptionManager,
                    logger: logger,
                    subscriptionClientName: config?.SubscriptionClientName ?? 
                                            configuration["ServiceBusSettings:SubscriptionClientName"],
                    processingPipeline: processingPipeline,
                    eventBusClient: eventBusClient);

                subscribeToEvents(eventProcessor);
                
                return eventProcessor;
            });
            
            services.AddSingleton<IMessageHandlingPipeline, MessageHandlingPipeline>();
            services.TryAddScoped<IMessagePreHandlingStep, ExtractTenantFromMessageMetadataStep>();
            services.TryAddScoped<IMessagePreHandlingStep, ExtractUserFromMessageMetadataStep>();

            services.AddHostedService<EventProcessorStartup>();

            registerHandlers(services);
        });

        return hostBuilder;
    }
}
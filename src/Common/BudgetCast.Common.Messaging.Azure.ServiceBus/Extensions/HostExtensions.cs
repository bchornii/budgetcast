using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;

// TODO: 1) create az service bus resource
// TODO: 2) create budgetcast_events_topic topic
// TODO: 3) add TestEvent correlation filter based on Subject message property
// TODO: 4) send message into topic - debug sending + receiving

public static class HostExtensions
{
    public static IHostBuilder UseAzServiceBus(
        this IHostBuilder hostBuilder, 
        Action<IServiceCollection> registerHandlers,
        Action<IEventsProcessor> subscribeToEvents,
        Action<ServiceBusConfiguration>? options = null)
    {
        hostBuilder.ConfigureServices((ctx, services) =>
        {
            ServiceBusConfiguration? config = null;
            if (options is not null)
            {
                config = new ServiceBusConfiguration();
                options(config);   
            }

            services.AddSingleton(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                return new EventBusClient(
                    connectionString: config?.AzureServiceBusConnectionString ?? 
                                      configuration["ServiceBusSettings:EventBusConnection"]);
            });
            
            services.AddScoped<IEventsPublisher, EventsPublisher>();
            services.AddScoped<IMessageSerializer, MessageSerializer>();
            
            services.AddSingleton<IEventsSubscriptionManager, EventsSubscriptionManager>();
            services.AddSingleton<IEventsProcessor, EventsProcessor>(provider =>
            {
                var subscriptionManager = provider.GetRequiredService<IEventsSubscriptionManager>();
                var logger = provider.GetRequiredService<ILogger<EventsProcessor>>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                var processingPipeline = provider.GetRequiredService<IMessageProcessingPipeline>();
                var eventBusClient = provider.GetRequiredService<EventBusClient>();
                var eventProcessor = new EventsProcessor(
                    subscriptionManager: subscriptionManager,
                    logger: logger,
                    subscriptionClientName: config?.SubscriptionClientName ?? 
                                            configuration["ServiceBusSettings:SubscriptionClientName"],
                    processingPipeline: processingPipeline,
                    eventBusClient: eventBusClient);

                subscribeToEvents(eventProcessor);
                
                return eventProcessor;
            });
            
            services.AddSingleton<IMessageProcessingPipeline, MessageProcessingPipeline>();
            services.TryAddScoped<IMessagePreProcessingStep, ExtractTenantFromMessageMetadataStep>();
            services.TryAddScoped<IMessagePreProcessingStep, ExtractUserFromMessageMetadataStep>();

            services.AddHostedService<EventsProcessorHostedService>();

            registerHandlers(services);
        });

        return hostBuilder;
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Extensions;

public class HostExtensionsTests
{
    private HostExtensionsFixture _fixture;

    public HostExtensionsTests()
    {
        _fixture = new HostExtensionsFixture();
    }

    #region Send messages only UseAzureServiceBus registration

    [Fact]
    public void UseAzureServiceBus_SendOnly_Should_Add_EventsPublisher_AsScoped()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus();

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(EventsPublisher) &&
                it.ServiceType == typeof(IEventsPublisher) &&
                it.Lifetime == ServiceLifetime.Scoped)));
    }
    
    [Fact]
    public void UseAzureServiceBus_SendOnly_Should_Add_MessageSerializer_AsScoped()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus();

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(MessageSerializer) &&
                it.ServiceType == typeof(IMessageSerializer) &&
                it.Lifetime == ServiceLifetime.Scoped)));
    }
    
    [Fact]
    public void UseAzureServiceBus_SendOnly_Should_Add_EventBusClient_AsSingleton()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus();

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ServiceType == typeof(IEventBusClient) &&
                it.Lifetime == ServiceLifetime.Singleton)));
    }

    [Fact]
    public void UseAzureServiceBus_SendOnly_Should_Populate_ServiceBusConfiguration()
    {
        // Arrange
        ServiceBusConfiguration cachedConfiguration = null!;
        var expectedConnectionString = _fixture.Fixture.Create<string>();
        var expectedSubscriptionClientName = _fixture.Fixture.Create<string>();
        
        // Act
        _fixture.HostBuilder.UseAzureServiceBus(configuration =>
        {
            configuration.SubscriptionClientName = expectedSubscriptionClientName;
            configuration.AzureServiceBusConnectionString = expectedConnectionString;
            cachedConfiguration = configuration;
        });

        // Assert
        cachedConfiguration
            .SubscriptionClientName
            .Should()
            .Be(expectedSubscriptionClientName);

        cachedConfiguration
            .AzureServiceBusConnectionString
            .Should()
            .Be(expectedConnectionString);
    }

    #endregion

    #region Send & process messages UseAzureServiceBus registration

    [Fact]
    public void UseAzureServiceBus_Should_Add_EventsPublisher_AsScoped()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus(
            registerHandlers: collection => { },
            subscribeToEvents: processor => { });

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(EventsPublisher) &&
                it.ServiceType == typeof(IEventsPublisher) &&
                it.Lifetime == ServiceLifetime.Scoped)));
    }
    
    [Fact]
    public void UseAzureServiceBus_Should_Add_MessageSerializer_AsScoped()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus(
            registerHandlers: collection => { },
            subscribeToEvents: processor => { });

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(MessageSerializer) &&
                it.ServiceType == typeof(IMessageSerializer) &&
                it.Lifetime == ServiceLifetime.Scoped)));
    }
    
    [Fact]
    public void UseAzureServiceBus_Should_Add_EventBusClient_AsSingleton()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus(
            registerHandlers: collection => { },
            subscribeToEvents: processor => { });

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ServiceType == typeof(IEventBusClient) &&
                it.Lifetime == ServiceLifetime.Singleton)));
    }
    
    [Fact]
    public void UseAzureServiceBus_Should_Add_EventsSubscriptionManager_AsSingleton()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus(
            registerHandlers: collection => { },
            subscribeToEvents: processor => { });

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(EventsSubscriptionManager) &&
                it.ServiceType == typeof(IEventsSubscriptionManager) &&
                it.Lifetime == ServiceLifetime.Singleton)));
    }
    
    [Fact]
    public void UseAzureServiceBus_Should_Add_EventsProcessor_AsSingleton()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus(
            registerHandlers: collection => { },
            subscribeToEvents: processor => { });

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ServiceType == typeof(IEventsProcessor) &&
                it.Lifetime == ServiceLifetime.Singleton)));
    }
    
    [Fact]
    public void UseAzureServiceBus_Pass_EventHandlers_Should_RegisterThem_AsScoped()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus(
            registerHandlers: collection =>
            {
                collection.AddScoped<FakeEventHandler1>();
                collection.AddScoped<FakeEventHandler2>();
            },
            subscribeToEvents: processor => { });

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(FakeEventHandler1) &&
                it.Lifetime == ServiceLifetime.Scoped)));
        
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(FakeEventHandler2) &&
                it.Lifetime == ServiceLifetime.Scoped)));
    }

    [Fact]
    public void UseAzureServiceBus_Should_Add_EventProcessingPipeline_AsSingleton()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus(
            registerHandlers: collection => { },
            subscribeToEvents: processor => { });

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(EventProcessingPipeline) &&
                it.ServiceType == typeof(IMessageProcessingPipeline) &&
                it.Lifetime == ServiceLifetime.Singleton)));
    }
    
    [Fact]
    public void UseAzureServiceBus_Should_Add_ExtractTenantFromMessageMetadataStep_AsScoped()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus(
            registerHandlers: collection => { },
            subscribeToEvents: processor => { });

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(ExtractTenantFromMessageMetadataStep) &&
                it.ServiceType == typeof(IMessagePreProcessingStep) &&
                it.Lifetime == ServiceLifetime.Scoped)));
    }
    
    [Fact]
    public void UseAzureServiceBus_Should_Add_ExtractUserFromMessageMetadataStep_AsScoped()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus(
            registerHandlers: collection => { },
            subscribeToEvents: processor => { });

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(ExtractUserFromMessageMetadataStep) &&
                it.ServiceType == typeof(IMessagePreProcessingStep) &&
                it.Lifetime == ServiceLifetime.Scoped)));
    }
    
    [Fact]
    public void UseAzureServiceBus_Should_Populate_ServiceBusConfiguration()
    {
        // Arrange
        ServiceBusConfiguration cachedConfiguration = null!;
        var expectedConnectionString = _fixture.Fixture.Create<string>();
        var expectedSubscriptionClientName = _fixture.Fixture.Create<string>();
        
        // Act
        _fixture.HostBuilder.UseAzureServiceBus(
            registerHandlers: collection => { },
            subscribeToEvents: processor => { },
            options: configuration =>
            {
                configuration.SubscriptionClientName = expectedSubscriptionClientName;
                configuration.AzureServiceBusConnectionString = expectedConnectionString;
                cachedConfiguration = configuration;
            });

        // Assert
        cachedConfiguration
            .SubscriptionClientName
            .Should()
            .Be(expectedSubscriptionClientName);

        cachedConfiguration
            .AzureServiceBusConnectionString
            .Should()
            .Be(expectedConnectionString);
    }

    #endregion

    private class HostExtensionsFixture
    {
        public Fixture Fixture { get; }
        
        public IHostBuilder HostBuilder { get; }
        
        public HostBuilderContext HostBuilderContext { get; }

        public IServiceCollection ServiceCollection { get; }

        public HostExtensionsFixture()
        {
            Fixture = new Fixture();
            HostBuilderContext = new HostBuilderContext(new Dictionary<object, object>());
            ServiceCollection = Mock.Of<IServiceCollection>();
            HostBuilder = new FakeHostBuilder(HostBuilderContext, ServiceCollection);
        }

        private class FakeHostBuilder : IHostBuilder
        {
            private readonly HostBuilderContext _context;
            private readonly IServiceCollection _collection;
            
            public FakeHostBuilder(HostBuilderContext context, IServiceCollection collection)
            {
                _context = context;
                _collection = collection;
                Properties = new Dictionary<object, object>();
            }
            
            public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
            {
                throw new NotImplementedException();
            }

            public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
            {
                return this;
            }

            public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
            {
                configureDelegate(_context, _collection);
                return this;
            }

            public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
            {
                return this;
            }

            public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
            {
                return this;
            }

            public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
            {
                return this;
            }

            public IHost Build()
            {
                return null!;
            }

            public IDictionary<object, object> Properties { get; }
        }
    }
}
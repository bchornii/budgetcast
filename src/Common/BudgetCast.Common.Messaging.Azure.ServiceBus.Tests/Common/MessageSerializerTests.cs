using System;
using AutoFixture;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Common;

public class MessageSerializerTests
{
    private readonly MessageSerializerFixture _fixture;

    public MessageSerializerTests()
    {
        _fixture = new MessageSerializerFixture();
    }

    [Fact]
    public void PackAsJson_IdentityCtx_DoesNot_Have_Tenant_Should_Set_It_On_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedTenant)
            .Returns(false);

        // Act
        var result = _fixture.Serializer.PackAsJson(integrationMessage);

        // Assert
        result
            .Should()
            .NotContain(IntegrationMessage.TenantIdMetadataKey);
    }
    
    [Fact]
    public void PackAsJson_IdentityCtx_Has_Tenant_Should_Set_It_On_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();

        var expectedTenantId = _fixture.Fixture.Create<long>();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedTenant)
            .Returns(true);

        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.TenantId)
            .Returns(expectedTenantId);

        // Act
        var result = _fixture.Serializer.PackAsJson(integrationMessage);

        // Assert
        result
            .Should()
            .Contain(IntegrationMessage.TenantIdMetadataKey);

        result
            .Should()
            .Contain(expectedTenantId.ToString());
    }
    
    [Fact]
    public void PackAsJson_IdentityCtx_And_Message_Both_Have_Tenant_Should_Not_Update_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();
        var messageTenantId = _fixture.Fixture.Create<long>();
        integrationMessage.SetCurrentTenant(messageTenantId);

        var ctxTenantId = _fixture.Fixture.Create<long>();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedTenant)
            .Returns(true);

        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.TenantId)
            .Returns(ctxTenantId);

        // Act
        var result = _fixture.Serializer.PackAsJson(integrationMessage);

        // Assert
        result
            .Should()
            .Contain(IntegrationMessage.TenantIdMetadataKey);

        result
            .Should()
            .Contain(messageTenantId.ToString());
    }
    
    [Fact]
    public void PackAsJson_IdentityCtx_DoesNot_Have_User_Should_Set_It_On_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedUser)
            .Returns(false);

        // Act
        var result = _fixture.Serializer.PackAsJson(integrationMessage);

        // Assert
        result
            .Should()
            .NotContain(IntegrationMessage.UserIdMetadataKey);
    }
    
    [Fact]
    public void PackAsJson_IdentityCtx_Has_User_Should_Set_It_On_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();

        var expectedUserId = _fixture.Fixture.Create<string>();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedUser)
            .Returns(true);

        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.UserId)
            .Returns(expectedUserId);

        // Act
        var result = _fixture.Serializer.PackAsJson(integrationMessage);

        // Assert
        result
            .Should()
            .Contain(IntegrationMessage.UserIdMetadataKey);

        result
            .Should()
            .Contain(expectedUserId);
    }
    
    [Fact]
    public void PackAsJson_IdentityCtx_And_Message_Both_Have_User_Should_Not_Update_Message()
    {
        // Arrange
        var integrationMessage = new FakeIntegrationMessage();
        var messageUserId = _fixture.Fixture.Create<string>();
        integrationMessage.SetUserId(messageUserId);
        
        var ctxUserId = _fixture.Fixture.Create<string>();
        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.HasAssociatedUser)
            .Returns(true);

        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.UserId)
            .Returns(ctxUserId);

        // Act
        var result = _fixture.Serializer.PackAsJson(integrationMessage);

        // Assert
        result
            .Should()
            .Contain(IntegrationMessage.UserIdMetadataKey);

        result
            .Should()
            .Contain(messageUserId);
    }

    [Fact]
    public void UnpackFromJson_Should_Initialize_All_IntegrationMessage_Properties()
    {
        // Arrange
        var json = MessageSerializerFixture.GetIntegrationMessageJson();

        // Act
        var result = _fixture.Serializer
            .UnpackFromJson(json, typeof(FakeIntegrationMessage)) as FakeIntegrationMessage;

        // Assert
        result!.Id
            .Should()
            .Be(Guid.Parse("7e7bb340-326f-2345-1234-f6cb65133b54"));

        result.CreatedAt
            .Should()
            .Be(DateTime.Parse("2022-03-04T00:00:00"));

        result.GetUserId()
            .Should()
            .Be("7e7bb340-326f-43aa-a65b-f6cb65133b54");

        result.GetTenantId()
            .Should()
            .Be(7643);
    }
    
    private class MessageSerializerFixture
    {
        public Fixture Fixture { get; }
        
        public IIdentityContext IdentityContext { get; }
        
        public ILogger<MessageSerializer> Logger { get; }
        
        public MessageSerializer Serializer { get; }

        public MessageSerializerFixture()
        {
            Fixture = new Fixture();
            IdentityContext = Mock.Of<IIdentityContext>();
            Logger = Mock.Of<ILogger<MessageSerializer>>();
            Serializer = new MessageSerializer(IdentityContext, Logger);
        }

        public static string GetIntegrationMessageJson()
            => "{\"Id\":\"7e7bb340-326f-2345-1234-f6cb65133b54\",\"CreatedAt\":\"2022-03-04T00:00:00\",\"Metadata\":{\"UserId\":\"7e7bb340-326f-43aa-a65b-f6cb65133b54\", \"TenantId\": \"7643\"}}";
    }

    private class FakeIntegrationMessage : IntegrationMessage
    {
    }
}
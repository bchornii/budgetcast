namespace BudgetCast.Common.Messaging.Abstractions.Common;

public interface IMessagePreProcessor
{
    string PackAsJson(IntegrationMessage message);

    object? UnpackFromJson(string message, Type messageType);
}
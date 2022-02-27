namespace BudgetCast.Common.Messaging.Abstractions.Common;

public interface IMessageSerializer
{
    string PackAsJson(IntegrationMessage message);

    object? UnpackFromJson(string message, Type messageType);
}
using MediatR;

namespace BudgetCast.Common.Domain;

/// <summary>
/// Represents domain event marker interface.
/// <remarks>Please note that domain events shall be handled in the same scope (aka. unit of work) as it's
/// entity.</remarks>
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccuredOn { get; }
}
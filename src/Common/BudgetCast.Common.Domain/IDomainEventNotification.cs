using MediatR;

namespace BudgetCast.Common.Domain;

/// <summary>
/// Represents domain event marker interface.
/// <remarks>Please note that domain events shall be handled outside it's entity scope (aka. another unit of work). It
/// designed purely for legacy scenarios -- where it's not possible to handle changes nor in the same UoW neither
/// by publishing integration event(s).
/// </remarks>
/// </summary>
public interface IDomainEventNotification : INotification
{
    DateTime AddedAt { get; }
}
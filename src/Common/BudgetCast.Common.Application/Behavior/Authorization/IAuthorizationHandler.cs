using BudgetCast.Common.Domain.Results;
using MediatR;

namespace BudgetCast.Common.Application.Behavior.Authorization;

/// <summary>
/// Authorization requirement handler
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IAuthorizationHandler<TRequest> : IRequestHandler<TRequest, Result<AuthorizationResult>>
    where TRequest : IRequest<Result<AuthorizationResult>>
{
}
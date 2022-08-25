using BudgetCast.Common.Domain.Results;
using MediatR;

namespace BudgetCast.Common.Application.Behavior.Authorization;

/// <summary>
/// Authorization requirement marker interface.
/// </summary>
public interface IAuthorizationRequirement : IRequest<Result<AuthorizationResult>> { }
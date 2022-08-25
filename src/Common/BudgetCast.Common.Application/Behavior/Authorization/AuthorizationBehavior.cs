using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Application.Behavior.Authorization;

/// <summary>
/// Runs authorization checks for queries and commands.
/// </summary>
/// <typeparam name="TRequest">Query or Command type</typeparam>
/// <typeparam name="TResponse">Response derived from <see cref="Result"/></typeparam>
public class AuthorizationBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public const string AuthFailedCode = "app.authorization";
    
    private readonly IEnumerable<IAuthorizer<TRequest>> _authorizers;
    private readonly IMediator _mediator;
    private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;

    public AuthorizationBehavior(
        IEnumerable<IAuthorizer<TRequest>> authorizers,
        IMediator mediator,
        ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
    {
        _authorizers = authorizers;
        _mediator = mediator;
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(
        TRequest request, 
        CancellationToken cancellationToken, 
        RequestHandlerDelegate<TResponse> next)
    {
        if (request.IsAuthorizationRequirement<TRequest, TResponse>())
        {
            return await next();
        }
        
        var requestName = typeof(TRequest).GetGenericTypeName();
        var requestType = request.GetRequestType<TRequest, TResponse>();
        
        var requirements = new List<IAuthorizationRequirement>();

        _logger.LogInformation("Authorizing {RequestName} {RequestType}", requestName, requestType);
        foreach (var authorizer in _authorizers)
        {
            authorizer.BuildPolicy(request);
            requirements.AddRange(authorizer.Requirements);
        }

        foreach (var requirement in requirements)
        {
            var requirementResult = await _mediator.Send(requirement, cancellationToken);
            if (!requirementResult.Value.IsAuthorized)
            {
                var error = new ValidationError(
                    AuthFailedCode, 
                    requirementResult.Value.FailureMessage);

                _logger.LogWarning("Authorization of {RequestName} {RequestType} failed!", requestName, requestType);
                
                if (typeof(TResponse).IsGenericResult())
                {
                    var genericArgumentType = typeof(TResponse)
                        .GetGenericResultArgumentType();
                    
                    var genericFailResult = typeof(Forbidden<>)
                        .CreateInstanceOf(genericArgumentType)
                        .WithErrors(error);

                    return (genericFailResult as TResponse)!;
                }

                var nonGenericFailResult = Result.Forbidden(error);
                return (nonGenericFailResult as TResponse)!;
            }
        }

        _logger.LogInformation("Authorization of {RequestName} {RequestType} succeeded.", requestName, requestType);
        return await next();
    }
}
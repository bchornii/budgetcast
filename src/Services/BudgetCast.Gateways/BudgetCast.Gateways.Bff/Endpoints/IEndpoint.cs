namespace BudgetCast.Gateways.Bff.Endpoints;

public record EndpointRequest
{
    public static readonly EndpointRequest Empty = new();
}

public interface IEndpoint
{
    void AddRoute(IEndpointRouteBuilder app);
}

public interface IEndpoint<in TRequest, TResult> : IEndpoint
    where TRequest : EndpointRequest
{
    Task<TResult> HandleAsync(TRequest request, CancellationToken token);
}
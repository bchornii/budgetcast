using BudgetCast.Common.Operations;
using Microsoft.AspNetCore.Http;

namespace BudgetCast.Common.Web.DelegationHandlers;

/// <summary>
/// Adds <see cref="OperationContext"/> into header with <see cref="OperationContext.MetaName"/> name.
/// If context is present is present in <see cref="HttpContext.Items"/> then it's used.
/// Otherwise new instance of <see cref="OperationContext"/> is created.
/// </summary>
public sealed class OperationHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OperationHeaderHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // HttpContext.Items is used as an intermediate storage for OperationContext in a scope
        // of a single request. See more details here:
        // https://andrewlock.net/understanding-scopes-with-ihttpclientfactory-message-handlers/#accessing-the-request-scope-from-a-custom-httpmessagehandler
        var isOperationContextSet = _httpContextAccessor.HttpContext.Items
            .ContainsKey(OperationContext.MetaName);

        var operationContext = isOperationContextSet
            ? (OperationContext)_httpContextAccessor.HttpContext.Items[OperationContext.MetaName]
            : OperationContext.New();

        if (!request.Headers.Contains(OperationContext.MetaName))
        {
            request.Headers.Add(OperationContext.MetaName, operationContext.Pack());
        }

        return base.SendAsync(request, cancellationToken);
    }
}
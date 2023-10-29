using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace BudgetCast.Gateways.Bff.TransformProviders;

public class LoggingTransformProvider : ITransformProvider
{
    private readonly ILogger<LoggingTransformProvider> _logger;

    public LoggingTransformProvider(ILogger<LoggingTransformProvider> logger) 
        => _logger = logger;

    public void ValidateRoute(TransformRouteValidationContext context)
    {
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
    }

    public void Apply(TransformBuilderContext context)
    {
        context.AddRequestTransform(async transformContext =>
        {
            //var token = await transformContext.HttpContext.GetUserAccessTokenAsync();
            //_logger.LogInformation("Propagation of Authorization: Bearer {0}", token);
        });
    }
}
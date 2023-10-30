using BudgetCast.Gateways.Bff.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace BudgetCast.Gateways.Bff.TransformProviders;

/// <summary>
/// If response is coming from <see cref="BffOptions.XTokenPath"/> then it's inspected for <see cref="BffOptions.XTokenHeaderName"/> to be
/// present as one of Set-Cookie items. If found it is parsed as JWT token. The token is used as a basis for authentication <see cref="BffOptions.BffAuthenticationCookieName"/> cookie generation.
/// </summary>
public class XTokenTransformProvider : ITransformProvider
{
    private readonly BffOptions _bffOptions;
    private readonly ILogger<XTokenTransformProvider> _logger;

    public XTokenTransformProvider(BffOptions bffOptions, ILogger<XTokenTransformProvider> logger)
    {
        _bffOptions = bffOptions;
        _logger = logger;
    }

    public void ValidateRoute(TransformRouteValidationContext context)
    {
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
    }

    public void Apply(TransformBuilderContext transformBuildContext)
    {
        transformBuildContext.AddResponseTransform(async responseTransformContext =>
        {
            var httpContext = responseTransformContext.HttpContext;
            
            if (httpContext.Request.Path.Equals(_bffOptions.XTokenPath))
            {
                var cookieValues = httpContext.Response.Headers.SetCookie.ToArray();

                if (cookieValues.Any<string>(cv => cv.StartsWith(_bffOptions.XTokenHeaderName)))
                {
                    var xToken = cookieValues.First<string>(cv => cv.StartsWith(_bffOptions.XTokenHeaderName));
                    var startIdx = xToken.IndexOf('=') + 1;
                    var len = xToken.IndexOf(';') - startIdx;
                    var accessToken = xToken.Substring(startIdx, len);
                    _logger.LogDebug("Extrificated an {accessToken} from the response to {url}", accessToken, httpContext.Request.GetDisplayUrl());

                    if (string.IsNullOrWhiteSpace(accessToken))
                    {
                        return;
                    }
                    
                    if (_bffOptions.RemoveXTokenCookieFromResponse)
                    {
                        _logger.LogInformation("Original {@setCookie} Set-Cookie values", cookieValues);
                        var nonXTokenCookieValues = cookieValues.Where<string>(cv => !cv.StartsWith(_bffOptions.XTokenHeaderName)).ToArray();
                        _logger.LogInformation("Modified {@setCookie} Set-Cookie values", nonXTokenCookieValues);
                        
                        httpContext.Response.Headers.Remove("Set-Cookie");
                        httpContext.Response.Headers.SetCookie = new StringValues(nonXTokenCookieValues);
                    }

                    await httpContext.SignInAsCookieAsync(accessToken);
                }
            }
        });
    }
}
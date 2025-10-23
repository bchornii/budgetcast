using BudgetCast.Gateways.Bff.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace BudgetCast.Gateways.Bff.TransformProviders;

/// <summary>
/// If response is coming from <see cref="BffOptions.XTokenPath"/> then it's inspected
/// for <see cref="BffOptions.XTokenHeaderName"/> to be present as one of Set-Cookie items.
/// If found it is parsed as JWT token. The token is used as a basis for authentication
/// <see cref="BffOptions.BffAuthenticationCookieName"/> cookie generation.
/// </summary>
public class XTokenTransformProvider(
    BffOptions bffOptions, 
    ILogger<XTokenTransformProvider> logger)
    : ITransformProvider
{
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
            var isExternalAuthentication = httpContext.Request.Path.Equals(bffOptions.XTokenPath);

            if (isExternalAuthentication)
            {
                var cookieValues = httpContext.Response.Headers.SetCookie.ToArray();

                if (cookieValues.Any<string>(cv => cv.StartsWith(bffOptions.XTokenHeaderName)))
                {
                    var xToken = cookieValues.First<string>(cv => cv.StartsWith(bffOptions.XTokenHeaderName));
                    var startIdx = xToken.IndexOf('=') + 1;
                    var len = xToken.IndexOf(';') - startIdx;
                    var accessToken = xToken.Substring(startIdx, len);
                    logger.LogDebug("Extrificated an {accessToken} from the response to {url}", accessToken, httpContext.Request.GetDisplayUrl());

                    if (string.IsNullOrWhiteSpace(accessToken))
                    {
                        return;
                    }
                    
                    if (bffOptions.RemoveXTokenCookieFromResponse)
                    {
                        logger.LogInformation("Original {@setCookie} Set-Cookie values", cookieValues);
                        var nonXTokenCookieValues = cookieValues.Where<string>(cv => !cv.StartsWith(bffOptions.XTokenHeaderName)).ToArray();
                        logger.LogInformation("Modified {@setCookie} Set-Cookie values", nonXTokenCookieValues);
                        
                        httpContext.Response.Headers.Remove("Set-Cookie");
                        httpContext.Response.Headers.SetCookie = new StringValues(nonXTokenCookieValues);
                    }

                    await httpContext.SignInAsCookieAsync(accessToken);
                }
            }
        });
    }
}
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using BudgetCast.Gateways.Bff.Models;
using BudgetCast.Gateways.Bff.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace BudgetCast.Gateways.Bff.TransformProviders;

public class AccessTokenTransformProvider : ITransformProvider
{
    private readonly ILogger<AccessTokenTransformProvider> _logger;

    public AccessTokenTransformProvider(ILogger<AccessTokenTransformProvider> logger) 
        => _logger = logger;

    public void ValidateRoute(TransformRouteValidationContext context)
    {
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
    }

    public void Apply(TransformBuilderContext transformBuildContext)
    {
        var routeValue = transformBuildContext.Route
            .Metadata?.GetValueOrDefault(AppConstants.Yarp.TokenTypeMetadata);
        var clusterValue = transformBuildContext.Cluster?
            .Metadata?.GetValueOrDefault(AppConstants.Yarp.TokenTypeMetadata);

        // no metadata
        if (string.IsNullOrEmpty(routeValue) && string.IsNullOrEmpty(clusterValue))
        {
            return;
        }
        
        var values = new HashSet<string>();
        if (!string.IsNullOrEmpty(routeValue)) values.Add(routeValue);
        if (!string.IsNullOrEmpty(clusterValue)) values.Add(clusterValue);

        if (values.Count > 1)
        {
            throw new ArgumentException(
                $"Mismatching {AppConstants.Yarp.TokenTypeMetadata} route or cluster metadata values found");
        }
        
        if (!Enum.TryParse(values.First(), true, out TokenType tokenType))
        {
            throw new ArgumentException($"Invalid value for {AppConstants.Yarp.TokenTypeMetadata} metadata");
        }
        
        transformBuildContext.AddRequestTransform(async transformContext =>
        {
            var tms = transformContext.HttpContext.RequestServices
                .GetRequiredService<IUserAccessTokenManagementService>();
            
            var token = await tms.GetUserAccessTokenAsync(transformContext.HttpContext.User);
            
            if (!string.IsNullOrWhiteSpace(token))
            {
                transformContext.ProxyRequest.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogError("Access token is missing on {0}", transformBuildContext.Route.RouteId);
            }
        });
    }
}
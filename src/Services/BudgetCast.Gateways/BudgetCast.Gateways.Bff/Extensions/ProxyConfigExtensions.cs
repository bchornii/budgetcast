using BudgetCast.Gateways.Bff.Models;
using Yarp.ReverseProxy.Configuration;

namespace BudgetCast.Gateways.Bff.Extensions;

public static class ProxyConfigExtensions
{
    public static RouteConfig WithAccessToken(this RouteConfig config, TokenType tokenType) 
        => config.WithMetadata(AppConstants.Yarp.TokenTypeMetadata, tokenType.ToString());

    public static RouteConfig WithAntiforgeryCheck(this RouteConfig config) 
        => config.WithMetadata(AppConstants.Yarp.AntiforgeryCheckMetadata, "true");

    private static RouteConfig WithMetadata(this RouteConfig config, string key, string value)
    {
        Dictionary<string, string> metadata;

        if (config.Metadata != null)
        {
            metadata = new Dictionary<string, string>(config.Metadata);
        }
        else
        {
            metadata = new();
        }

        metadata.TryAdd(key, value);
            
        return config with { Metadata = metadata };
    }
}
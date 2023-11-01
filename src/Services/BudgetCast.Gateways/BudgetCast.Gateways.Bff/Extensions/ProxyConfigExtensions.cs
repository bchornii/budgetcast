using BudgetCast.Gateways.Bff.Models;
using Yarp.ReverseProxy.Configuration;

namespace BudgetCast.Gateways.Bff.Extensions;

public static class ProxyConfigExtensions
{
    public static RouteConfig WithAccessToken(this RouteConfig config, TokenType tokenType)
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

        metadata.TryAdd(AppConstants.Yarp.TokenTypeMetadata, tokenType.ToString());
            
        return config with { Metadata = metadata };
    }
}
using BudgetCast.Gateways.Bff.Models;
using Yarp.ReverseProxy.Configuration;

namespace BudgetCast.Gateways.Bff.Extensions;

public static class ReverseProxyBuilderExtensions
{
    public static void BuildApplicationRoutes(this IReverseProxyBuilder builder, AppSettings appSettings)
    {
        builder.LoadFromMemory(
            routes: new[]
            {
                // Expenses API
                new RouteConfig
                {
                    RouteId = "expenses",
                    ClusterId = "expenses-api",

                    Match = new RouteMatch()
                    {
                        Path = "/api/expenses/{**catch-all}"
                    }
                }.WithAccessToken(TokenType.User),
                new RouteConfig
                {
                    RouteId = "campaigns",
                    ClusterId = "expenses-api",

                    Match = new RouteMatch
                    {
                        Path = "/api/campaigns/{**catch-all}"
                    }
                }.WithAccessToken(TokenType.User),
                
                // Identity API
                new RouteConfig
                {
                    RouteId = "accounts-signed-in",
                    ClusterId = "identity-api",
                    
                    Match = new RouteMatch
                    {
                        Path = "/api/account/update"
                    }
                }.WithAccessToken(TokenType.User),
                new RouteConfig
                {
                    RouteId = "accounts-is-authenticated",
                    ClusterId = "identity-api",
                    
                    Match = new RouteMatch
                    {
                        Path = "/api/account/isAuthenticated"
                    }
                }.WithAccessToken(TokenType.User),
                new RouteConfig
                {
                    RouteId = "accounts-anonymous",
                    ClusterId = "identity-api",
                    
                    Match = new RouteMatch
                    {
                        Path = "/api/account/{**catch-all}"
                    }
                },
                new RouteConfig
                {
                    RouteId = "ext-logins",
                    ClusterId = "identity-api",

                    Match = new RouteMatch
                    {
                        Path = "/api/signin/{**catch-all}"
                    }
                },
                new RouteConfig
                {
                    RouteId = "google-callback",
                    ClusterId = "identity-api",

                    Match = new RouteMatch
                    {
                        Path = "/g-callback/{**catch-all}"
                    }
                },
                new RouteConfig
                {
                    RouteId = "facebook-logins",
                    ClusterId = "identity-api",

                    Match = new RouteMatch
                    {
                        Path = "/fb-callback/{**catch-all}"
                    }
                }
            },
            clusters: new[]
            {
                new ClusterConfig
                {
                    ClusterId = "expenses-api",
                    Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "destination1", new DestinationConfig() { Address = appSettings.ExpensesApiUrl } }
                    }
                },
                new ClusterConfig
                {
                    ClusterId = "identity-api",
                    Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "destination1", new DestinationConfig() { Address = appSettings.IdentityApiUrl } }
                    }
                }
            });
    }
}
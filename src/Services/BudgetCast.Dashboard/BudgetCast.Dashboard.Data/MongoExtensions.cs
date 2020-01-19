using Microsoft.Extensions.DependencyInjection;

namespace BudgetCast.Dashboard.Data
{
    public static class MongoExtensions
    {
        public static IServiceCollection AddMongoMaps(this IServiceCollection services)
        {
            MongoMapsRegistrator.RegisterDocumentMaps();
            return services;
        }
    }
}

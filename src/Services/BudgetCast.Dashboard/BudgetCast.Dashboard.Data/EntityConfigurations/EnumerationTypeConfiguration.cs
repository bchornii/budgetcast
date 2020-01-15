using BudgetCast.Dashboard.Domain.SeedWork;
using MongoDB.Bson.Serialization;

namespace BudgetCast.Dashboard.Data.EntityConfigurations
{
    public class EnumerationTypeConfiguration
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<Enumeration>(config =>
            {
                config.AutoMap();
            });
        }
    }
}
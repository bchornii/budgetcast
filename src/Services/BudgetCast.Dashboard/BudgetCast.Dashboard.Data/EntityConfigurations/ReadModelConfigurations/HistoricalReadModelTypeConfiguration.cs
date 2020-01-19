using System;
using BudgetCast.Dashboard.Domain.SeedWork;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BudgetCast.Dashboard.Data.EntityConfigurations.ReadModelConfigurations
{
    public class HistoricalReadModelTypeConfiguration
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<HistoricalReadModel>(config =>
            {
                config.MapProperty(t => t.CreatedBy).SetElementName("CreatedBy");
                config.MapProperty(t => t.CreatedAt).SetElementName("CreatedAt")
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Local));

                config.MapProperty(t => t.UpdatedBy).SetElementName("UpdatedBy");
                config.MapProperty(t => t.UpdatedAt).SetElementName("UpdatedAt")
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
            });
        }
    }
}
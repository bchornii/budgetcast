using System;
using BudgetCast.Dashboard.Domain.SeedWork;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BudgetCast.Dashboard.Data.EntityConfigurations
{
    public class AggregateRootTypeConfiguration
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<AggregateRoot>(config =>
            {
                config.MapField("_createdBy").SetElementName("CreatedBy");
                config.MapField("_createdAt").SetElementName("CreatedAt")
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Local));

                config.MapField("_updatedBy").SetElementName("UpdatedBy");
                config.MapField("_updatedAt").SetElementName("UpdatedAt")
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
            });
        }
    }
}
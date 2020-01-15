using System;
using BudgetCast.Dashboard.Domain.Aggregates.Campaigns;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BudgetCast.Dashboard.Data.EntityConfigurations
{
    public class CampaignEntityTypeConfiguration
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<Campaign>(config =>
            {
                config.AutoMap();
                config.SetIgnoreExtraElements(true);

                config.MapField("_name").SetElementName("Name");
                config.MapField("_startsAt").SetElementName("StartsAt")
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
                config.MapField("_completesAt").SetElementName("CompletesAt")
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
            });
        }
    }
}
using System;
using BudgetCast.Dashboard.Domain.Aggregates.Receipting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BudgetCast.Dashboard.Data.EntityConfigurations
{
    public class ReceiptEntityTypeConfiguration
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<Receipt>(config =>
            {
                config.AutoMap();
                config.SetIgnoreExtraElements(true);

                config.MapField("_date").SetElementName("Date")
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
                config.MapField("_description").SetElementName("Description");
                config.MapField("_campaignId").SetElementName("CampaignId");
                config.MapField("_receiptItems").SetElementName("ReceiptItems");
                config.MapField("_tags").SetElementName("Tags");
            });

            BsonClassMap.RegisterClassMap<ReceiptItem>(config =>
            {
                config.AutoMap();
                config.SetIgnoreExtraElements(true);

                config.MapField("_title").SetElementName("Title");
                config.MapField("_description").SetElementName("Description");
                config.MapField("_price").SetElementName("Price")
                    .SetSerializer(new DecimalSerializer(BsonType.Decimal128));
                config.MapField("_quantity").SetElementName("Quantity");
            });
        }
    }
}

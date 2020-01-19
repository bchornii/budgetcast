using System;
using BudgetCast.Dashboard.Domain.ReadModel.Receipts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BudgetCast.Dashboard.Data.EntityConfigurations.ReadModelConfigurations
{
    public class ReceiptItemReadModelTypeConfiguration : MongoDbClassMap<ReceiptItem>
    {
        public override void Map(BsonClassMap<ReceiptItem> config)
        {
            config.AutoMap();
            config.SetIgnoreExtraElements(true);

            config.MapProperty(t => t.Title).SetElementName("Title");
            config.MapProperty(t => t.Description).SetElementName("Description");
            config.MapProperty(t => t.Price).SetElementName("Price")
                .SetSerializer(new DecimalSerializer(BsonType.Decimal128));
            config.MapProperty(t => t.Quantity).SetElementName("Quantity");
        }
    }

    public class ReceiptReadModelTypeConfiguration : MongoDbClassMap<Receipt>
    {
        public override void Map(BsonClassMap<Receipt> config)
        {
            config.AutoMap();
            config.SetIgnoreExtraElements(true);

            config.MapProperty(t => t.Date).SetElementName("Date")
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
            config.MapProperty(t => t.Description).SetElementName("Description");
            config.MapProperty(t => t.CampaignId).SetElementName("CampaignId");
            config.MapProperty(t => t.ReceiptItems).SetElementName("ReceiptItems");
            config.MapProperty(t => t.Tags).SetElementName("Tags");
        }
    }
}

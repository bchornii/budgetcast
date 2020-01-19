using BudgetCast.Dashboard.Domain.SeedWork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace BudgetCast.Dashboard.Data.EntityConfigurations.ReadModelConfigurations
{
    public class IdentifiableReadModelTypeConfiguration
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<IdentifiableReadModel>(config =>
            {
                config.MapIdMember(m => m.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance);
                config.IdMemberMap
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }
    }
}
using BudgetCast.Dashboard.Domain.SeedWork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace BudgetCast.Dashboard.Data.EntityConfigurations.ReadModelConfigurations
{
    public class IdentifiableReadModelTypeConfiguration : MongoDbClassMap<IdentifiableReadModel>
    {
        public override void Map(BsonClassMap<IdentifiableReadModel> config)
        {
            config.MapIdMember(m => m.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance);
            config.IdMemberMap
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
        }
    }
}
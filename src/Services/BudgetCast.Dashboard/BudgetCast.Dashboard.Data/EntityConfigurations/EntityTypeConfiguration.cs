using BudgetCast.Dashboard.Domain.SeedWork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace BudgetCast.Dashboard.Data.EntityConfigurations
{
    public class EntityTypeConfiguration : MongoDbClassMap<Entity>
    {
        public override void Map(BsonClassMap<Entity> config)
        {
            config.MapIdMember(m => m.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance);
            config.IdMemberMap
                .SetSerializer(new StringSerializer(BsonType.ObjectId));

            config.UnmapMember(e => e.DomainEvents);
        }
    }
}

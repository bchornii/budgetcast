using BudgetCast.Dashboard.Domain.SeedWork;
using MongoDB.Bson.Serialization;

namespace BudgetCast.Dashboard.Data.EntityConfigurations
{
    public class EnumerationTypeConfiguration : MongoDbClassMap<Enumeration>
    {
        public override void Map(BsonClassMap<Enumeration> config)
        {
            config.AutoMap();
        }
    }
}
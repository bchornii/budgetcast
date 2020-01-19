using MongoDB.Bson.Serialization;

namespace BudgetCast.Dashboard.Data
{
    public abstract class MongoDbClassMap<T>
    {
        protected MongoDbClassMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>(Map);
            }
        }

        public abstract void Map(BsonClassMap<T> config);
    }
}

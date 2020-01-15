using MongoDB.Bson;

namespace BudgetCast.Dashboard.Domain.Factories
{
    public class UniqueIdFactory
    {
        public static string GetId() => ObjectId.GenerateNewId().ToString();
    }
}

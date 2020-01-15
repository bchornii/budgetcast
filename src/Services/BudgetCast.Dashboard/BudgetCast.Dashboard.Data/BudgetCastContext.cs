using System.Linq;
using System.Reflection;
using BudgetCast.Dashboard.Domain.Aggregates.Campaigns;
using BudgetCast.Dashboard.Domain.Aggregates.Receipting;
using BudgetCast.Dashboard.Domain.SeedWork;
using MediatR;
using MongoDB.Driver;

namespace BudgetCast.Dashboard.Data
{
    public class BudgetCastContext
    {
        public MongoDbSet<Receipt> Receipts { get; }
        public MongoDbSet<Campaign> Campaigns { get; }

        public BudgetCastContext(string connectionString, IMediator mediator, string userId)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("BudgetCastDashboard");

            Receipts = new MongoDbSet<Receipt>(
                database.GetCollection<Receipt>(nameof(Receipts)), mediator, userId);

            Campaigns = new MongoDbSet<Campaign>(
                database.GetCollection<Campaign>(nameof(Campaigns)), mediator, userId);
        }

        public MongoDbSet<T> GetDbSet<T>() where T : AggregateRoot
        {
            return GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(p => p.PropertyType == typeof(MongoDbSet<T>))
                ?.GetValue(this) as MongoDbSet<T>;
        }
    }
}

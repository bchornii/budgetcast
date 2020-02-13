using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using BudgetCast.Dashboard.Domain.Aggregates.Campaigns;
using BudgetCast.Dashboard.Domain.Aggregates.Receipting;
using BudgetCast.Dashboard.Domain.AnemicModel;
using BudgetCast.Dashboard.Domain.SeedWork;
using MediatR;
using MongoDB.Driver;

namespace BudgetCast.Dashboard.Data
{
    public class BudgetCastContext
    {
        #region Anemic models
        public IMongoCollection<DefaultTag> DefaultTags { get; }
        #endregion

        #region Read models
        public IMongoCollection<Domain.ReadModel.Receipts.Receipt> ReceiptsCollection { get; set; }
        public IMongoCollection<Domain.ReadModel.Campaigns.Campaign> CampaignsCollection { get; set; }
        #endregion

        #region Entity models
        public MongoDbSet<Receipt> Receipts { get; }
        public MongoDbSet<Campaign> Campaigns { get; }
        #endregion

        public BudgetCastContext(string connectionString, IMediator mediator, string userId)
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings
            {
                EnabledSslProtocols = SslProtocols.Tls12
            };
            var mongoClient = new MongoClient(settings);
            var database = mongoClient.GetDatabase("BudgetCastDashboard");

            Receipts = new MongoDbSet<Receipt>(
                database.GetCollection<Receipt>(nameof(Receipts)), mediator, userId);

            Campaigns = new MongoDbSet<Campaign>(
                database.GetCollection<Campaign>(nameof(Campaigns)), mediator, userId);

            DefaultTags = database.GetCollection<DefaultTag>(nameof(DefaultTags));

            ReceiptsCollection = database.GetCollection<Domain.ReadModel.Receipts.Receipt>(nameof(Receipts));

            CampaignsCollection = database.GetCollection<Domain.ReadModel.Campaigns.Campaign>(nameof(Campaigns));
        }

        public MongoDbSet<T> GetDbSet<T>() where T : AggregateRoot
        {
            return GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(p => p.PropertyType == typeof(MongoDbSet<T>))
                ?.GetValue(this) as MongoDbSet<T>;
        }
    }
}

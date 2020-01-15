using Autofac;
using BudgetCast.Dashboard.Domain.Aggregates.Campaigns;
using BudgetCast.Dashboard.Domain.Aggregates.Receipting;
using BudgetCast.Dashboard.Domain.ReadModel.Campaign;
using BudgetCast.Dashboard.ReadAccessors;
using BudgetCast.Dashboard.Repository;

namespace BudgetCast.Dashboard.Api.Infrastructure.AutofacModules
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterRepositories(builder);
            RegisterReadAccessors(builder);
        }

        private static void RegisterReadAccessors(ContainerBuilder builder)
        {
            builder.RegisterType<CampaignReadAccessor>()
                .As<ICampaignReadAccessor>()
                .InstancePerLifetimeScope();
        }

        private static void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<ReceiptRepository>()
                .As<IReceiptRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CampaignRepository>()
                .As<ICampaignRepository>()
                .InstancePerLifetimeScope();
        }
    }
}

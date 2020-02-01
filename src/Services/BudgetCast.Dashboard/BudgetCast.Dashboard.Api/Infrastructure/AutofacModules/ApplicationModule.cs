using Autofac;
using BudgetCast.Dashboard.Domain.Aggregates.Campaigns;
using BudgetCast.Dashboard.Domain.Aggregates.Receipting;
using BudgetCast.Dashboard.Domain.AnemicModel;
using BudgetCast.Dashboard.Domain.ReadModel.Campaigns;
using BudgetCast.Dashboard.Domain.ReadModel.Receipts;
using BudgetCast.Dashboard.Domain.ReadModel.Tags;
using BudgetCast.Dashboard.ReadAccessors;
using BudgetCast.Dashboard.Repository;
using BudgetCast.Dashboard.WriteAccessors;

namespace BudgetCast.Dashboard.Api.Infrastructure.AutofacModules
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterRepositories(builder);
            RegisterReadAccessors(builder);
            RegisterWriteAccessors(builder);
        }

        private void RegisterWriteAccessors(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultTagWriteAccessor>()
                .As<IDefaultTagWriteAccessor>()
                .InstancePerLifetimeScope();
        }

        private static void RegisterReadAccessors(ContainerBuilder builder)
        {
            builder.RegisterType<CampaignReadAccessor>()
                .As<ICampaignReadAccessor>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DefaultTagReadAccessor>()
                .As<IDefaultTagReadAccessor>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ReceiptReadAccessor>()
                .As<IReceiptReadAccessor>()
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

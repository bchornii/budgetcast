using Autofac;
using BudgetCast.Dashboard.Api.Compensations;
using BudgetCast.Dashboard.Api.Infrastructure.AppSettings;
using BudgetCast.Dashboard.Api.Infrastructure.ExecutionHistoryStores;
using BudgetCast.Dashboard.Api.Services;
using BudgetCast.Dashboard.Blobs;
using BudgetCast.Dashboard.Compensations;
using BudgetCast.Dashboard.Domain.Aggregates.Campaigns;
using BudgetCast.Dashboard.Domain.Aggregates.Receipting;
using BudgetCast.Dashboard.Domain.AnemicModel;
using BudgetCast.Dashboard.Domain.Blobs;
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
        private readonly AzBlobStorageSettings _blobStorageSettings;
        private readonly UploadFileSettings _uploadFileSettings;
        private readonly AzBlobStorageContainersSettings _containersSettings;

        public ApplicationModule(
            AzBlobStorageContainersSettings containersSettings, 
            AzBlobStorageSettings blobStorageSettings,
            UploadFileSettings uploadFileSettings)
        {
            _containersSettings = containersSettings;
            _blobStorageSettings = blobStorageSettings;
            _uploadFileSettings = uploadFileSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterRepositories(builder);
            RegisterReadAccessors(builder);
            RegisterWriteAccessors(builder);
            RegisterBlobDataServices(builder);
            RegisterCompensations(builder);
        }

        private void RegisterCompensations(ContainerBuilder builder)
        {
            builder.RegisterType<CompensationActionsFactory>()
                .As<ICompensationActionsFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UploadProfileImageCompensation>()
                .As<ICompensationAction>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ExecutionHistoryStore>()
                .As<IExecutionHistoryStore>()
                .InstancePerLifetimeScope();
        }

        private void RegisterBlobDataServices(ContainerBuilder builder)
        {
            builder.RegisterType<ProfileBlobDataService>()
                .As<IProfileBlobDataService>()
                .WithParameter("connectionString", _blobStorageSettings.ConnectionString)
                .WithParameter("containerName", _containersSettings.UserProfile)
                .InstancePerLifetimeScope();

            builder.RegisterType<FileStreamReader>()
                .WithParameter("sizeLimit", _uploadFileSettings.SizeLimit)
                .WithParameter("permittedExtensions", _uploadFileSettings.PermittedExtensions)
                .As<IFileStreamReader>()
                .InstancePerLifetimeScope();
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

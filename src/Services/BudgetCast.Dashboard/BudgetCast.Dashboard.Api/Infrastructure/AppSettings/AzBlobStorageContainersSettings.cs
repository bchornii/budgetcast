namespace BudgetCast.Dashboard.Api.Infrastructure.AppSettings
{
    public class AzBlobStorageContainersSettings
    {
        public string UserProfile { get; set; }

        public string[] Containers => new[] { UserProfile };
    }
}
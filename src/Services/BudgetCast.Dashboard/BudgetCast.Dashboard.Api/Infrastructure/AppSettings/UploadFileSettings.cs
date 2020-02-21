namespace BudgetCast.Dashboard.Api.Infrastructure.AppSettings
{
    public class UploadFileSettings
    {
        public long SizeLimit { get; set; }
        public string[] PermittedExtensions { get; set; }
    }
}
namespace BudgetCast.Common.Web.HostedServices;

public class OperationsRegistryOptions
{
    public bool EnableCleanup { get; set; }

    public DateTime CleanupJobRunTime { get; set; }
}
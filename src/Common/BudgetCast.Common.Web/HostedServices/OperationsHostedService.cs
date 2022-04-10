using BudgetCast.Common.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BudgetCast.Common.Web.HostedServices;

public sealed class OperationsHostedService : IHostedService, IDisposable
{
    private readonly IServiceProvider _services;
    private readonly IOptionsMonitor<OperationsRegistryOptions> _options;
    private readonly ILogger<OperationsHostedService> _logger;
    private readonly CancellationTokenSource _stoppingCts = new();
    private readonly string _serviceName;

    private Timer _timer;
    private bool _disposed;

    public OperationsHostedService(
        IServiceProvider services,
        IConfiguration globalConfiguration,
        IOptionsMonitor<OperationsRegistryOptions> options,
        ILogger<OperationsHostedService> logger)
    {
        _services = services;
        _options = options;
        _logger = logger;
        _timer = default!;
        _serviceName = globalConfiguration["ServiceName"];

        _options.OnChange(o => SetTimer());
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scheduling operation registry cleanup by {ServiceName}", _serviceName);

        SetTimer();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Change(Timeout.Infinite, 0);
        _stoppingCts?.Cancel();
        _logger.LogWarning("Stopped operation registry cleanup by {ServiceName}", _serviceName);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _timer?.Dispose();
                _stoppingCts?.Cancel();
                _stoppingCts?.Dispose();
            }

            _disposed = true;
        }
    }

    private async Task CleanAsync()
    {
        try
        {
            using var scope = _services.CreateScope();
            var operationsDal = scope.ServiceProvider.GetRequiredService<IOperationsDal>();

            _logger.LogInformation("Starting cleaning operation at: {time}", DateTimeOffset.UtcNow);
            var cleanedTableName = await operationsDal.CleanAsync(_stoppingCts.Token);
            _logger.LogInformation("Cleaning operation finished");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operations hosted service failed to truncate operations table");
        }
        finally
        {
            _logger.LogInformation("Rescheduling operation registry cleanup by {ServiceName}", _serviceName);
            SetTimer();
        }
    }

    private void SetTimer()
    {
        if (_options.CurrentValue.EnableCleanup)
        {
            var startsIn = CalculateStartsInTime();

            if (_timer == null)
            {
                _timer = new Timer(ExecuteTask, null, startsIn, Timeout.InfiniteTimeSpan);
            }
            else
            {
                _timer.Change(startsIn, Timeout.InfiniteTimeSpan);
            }

            _logger.LogInformation("Operation registry cleanup starts in {StartsIn}", startsIn);
        }
        else
        {
            _logger.LogWarning("Operation registry cleanup is turned off");
        }
    }
    
    private void ExecuteTask(object state)
    {
        CleanAsync();
    }

    private TimeSpan CalculateStartsInTime()
        => DateTime.Compare(SystemDt.Current, _options.CurrentValue.CleanupJobRunTime) < 0
            ? _options.CurrentValue.CleanupJobRunTime.Subtract(SystemDt.Current)
            : _options.CurrentValue.CleanupJobRunTime.AddDays(1).Subtract(SystemDt.Current);
}

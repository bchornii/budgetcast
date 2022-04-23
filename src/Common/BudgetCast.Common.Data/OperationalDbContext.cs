using System.Data;
using BudgetCast.Common.Application.Outbox;
using BudgetCast.Common.Data.EventLog;
using BudgetCast.Common.Data.OperationRegistry;
using BudgetCast.Common.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BudgetCast.Common.Data;

public class OperationalDbContext : DbContext
{
    private IDbContextTransaction? _currentTransaction;
    
    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;
    
    public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }
    
    public DbSet<OperationRegistryEntry> OperationRegistryEntries { get; set; }

    public bool HasActiveTransaction => _currentTransaction != null;
    
    protected virtual string DbSchema => "dbo";

    protected virtual string OperationRegistryTableName => "OperationsRegistry";

    protected virtual string IntegrationEventLogEntryTableName => "IntegrationEventLog";

    public OperationalDbContext(DbContextOptions options) 
        : base(options)
    {
        IntegrationEventLogs = Set<IntegrationEventLogEntry>();
        OperationRegistryEntries = Set<OperationRegistryEntry>();
    }
    
    public async Task<IDbContextTransaction?> BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            return null;
        }

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        return _currentTransaction;
    }
    
    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        if (transaction != _currentTransaction)
        {
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");
        }

        try
        {
            await base.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder
            .ApplyConfiguration(new OperationRegistryEntityTypeConfiguration(OperationRegistryTableName, DbSchema))
            .ApplyConfiguration(new IntegrationEventLogEntryTypeConfiguration(IntegrationEventLogEntryTableName, DbSchema));
    }
    
    private void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
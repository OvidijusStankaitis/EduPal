using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PSI_Project.Interceptor;

public class AuditingInterceptor : ISaveChangesInterceptor
{
    private readonly ILogger<AuditingInterceptor> _logger;
    private SaveChangesAudit _audit;

    public AuditingInterceptor(ILogger<AuditingInterceptor> logger)
    {
        _logger = logger;
    }

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _audit = CreateAudit(eventData.Context);
        LogAudit(_audit);

        return result;
    }

    public InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        _audit = CreateAudit(eventData.Context);
        LogAudit(_audit);

        return result;
    }

    public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        _audit.Succeeded = true;
        _audit.EndTime = DateTime.UtcNow;
        LogAuditResult(_audit);

        return result;
    }

    public async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        _audit.Succeeded = true;
        _audit.EndTime = DateTime.UtcNow;
        LogAuditResult(_audit);

        return result;
    }

    public void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _audit.Succeeded = false;
        _audit.EndTime = DateTime.UtcNow;
        _audit.ErrorMessage = eventData.Exception.Message;
        LogAuditResult(_audit);
    }

    public async Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _audit.Succeeded = false;
        _audit.EndTime = DateTime.UtcNow;
        _audit.ErrorMessage = eventData.Exception.InnerException?.Message;
        LogAuditResult(_audit);
    }

    private static SaveChangesAudit CreateAudit(DbContext context)
    {
        context.ChangeTracker.DetectChanges();

        var audit = new SaveChangesAudit { AuditId = Guid.NewGuid(), StartTime = DateTime.UtcNow };

        foreach (var entry in context.ChangeTracker.Entries())
        {
            var auditMessage = entry.State switch
            {
                EntityState.Deleted => CreateDeletedMessage(entry),
                EntityState.Modified => CreateModifiedMessage(entry),
                EntityState.Added => CreateAddedMessage(entry),
                _ => null
            };

            if (auditMessage != null)
            {
                audit.Entities.Add(new EntityAudit { State = entry.State, AuditMessage = auditMessage });
            }
        }

        return audit;
        
        string CreateAddedMessage(EntityEntry entry)
            => entry.Properties.Aggregate(
                $"Inserting {entry.Metadata.DisplayName()} with ",
                (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");

        string CreateModifiedMessage(EntityEntry entry)
            => entry.Properties.Where(property => property.IsModified || property.Metadata.IsPrimaryKey()).Aggregate(
                $"Updating {entry.Metadata.DisplayName()} with ",
                (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");

        string CreateDeletedMessage(EntityEntry entry)
            => entry.Properties.Where(property => property.Metadata.IsPrimaryKey()).Aggregate(
                $"Deleting {entry.Metadata.DisplayName()} with ",
                (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");
    }

    private void LogAudit(SaveChangesAudit audit)
    {
        _logger.LogInformation($"Audit started at {audit.StartTime}.");

        foreach (var entity in audit.Entities)
        {
            _logger.LogInformation($"  {entity.AuditMessage}");
        }
    }

    private void LogAuditResult(SaveChangesAudit audit)
    {
        if (audit.Succeeded)
        {
            _logger.LogInformation($"Audit completed successfully at {audit.EndTime}.");
        }
        else
        {
            _logger.LogError($"Audit failed at {audit.EndTime}. Error: {audit.ErrorMessage}");
        }
    }
}

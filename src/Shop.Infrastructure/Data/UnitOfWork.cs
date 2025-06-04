using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shop.Core.SharedKernel;

namespace Shop.Infrastructure.Data;

internal sealed class UnitOfWork(
    IMiraEventStore miraEventStore,
    ILogger<UnitOfWork> logger) : IUnitOfWork
{
    /// <summary>
    /// Saves changes asynchronously.
    /// </summary>
    public async Task SaveChangesAsync(BaseEntity entity)
    {
        try
        {
            var domainEvents = entity.DomainEvents.ToList().MaxBy(e => e.Version);
            await SaveEventsAsync([domainEvents]);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An unexpected exception occurred while saving events");
            throw;
        }
    }


    /// <summary>
    /// Performs necessary actions after saving changes, such as publishing domain events and storing event stores.
    /// </summary>
    /// <param name="domainEvents">The list of domain events.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    //private async Task AfterSaveChangesAsync(IReadOnlyList<BaseEvent> domainEvents, IReadOnlyList<EventStore_> eventStores)
    private async Task SaveEventsAsync(IReadOnlyList<BaseEvent> domainEvents)
    {
        // 2. Persist to EventStoreDB – группируем по AggregateId
        foreach (var grp in domainEvents.GroupBy(e => e.AggregateId))
        {
            await miraEventStore.SaveEventsAsync(grp.Key, grp);
        }
    }

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~UnitOfWork() => Dispose(false);

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        // Dispose managed state (managed objects).
        if (disposing)
        {
            //eventStoreRepository.Dispose();
        }

        _disposed = true;
    }

    #endregion
}
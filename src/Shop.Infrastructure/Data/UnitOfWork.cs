using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shop.Core.SharedKernel;
using Shop.Domain;
using Shop.Domain.Entities.CustomerAggregate;

namespace Shop.Infrastructure.Data;

internal sealed class UnitOfWork(
    IMiraEventStore miraEventStore,
    IMiraSnapshotRepository miraSnapshotRepository,
    ILogger<UnitOfWork> logger) : IUnitOfWork
{
    /// <summary>
    /// Saves changes asynchronously.
    /// </summary>
    public async Task SaveChangesAsync(BaseEntity entity)
    {
        try
        {
            var domainEvents = entity.DomainEvents.ToList();
            await SaveEventsAsync(domainEvents);
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
        var newDomainEvent = domainEvents.MaxBy(e => e.Version);
        //сохраняем только новое событие
        await miraEventStore.SaveEventsAsync(newDomainEvent.AggregateId, [newDomainEvent]);

        if (newDomainEvent.Version % 5 == 0)
            miraSnapshotRepository.SaveSnapshotsAsync(domainEvents);
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
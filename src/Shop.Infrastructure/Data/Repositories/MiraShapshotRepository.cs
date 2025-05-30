using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MongoDB.Driver.Linq;
using Shop.Core.SharedKernel;
using Shop.Domain;
using Shop.Domain.Entities.CustomerAggregate;

namespace Shop.Infrastructure.Data.Repositories;

public class MiraSnapshotRepository : IMiraSnapshotRepository
{
    private static readonly Dictionary<Guid, Customer> CustomersSnapshots = new();

    public void SaveSnapshotsAsync(IReadOnlyList<BaseEvent> domainEvents)
    {
        var customer = GetLastSnapshot(domainEvents.FirstOrDefault()!.AggregateId)
                       ?? new Customer();

        customer.Replay(domainEvents);
        CustomersSnapshots[customer.Id] = customer;
    }

    [CanBeNull]
    public Customer GetLastSnapshot(Guid customerId)
    {
        if (CustomersSnapshots.TryGetValue(customerId, out var snapshot))
        {
            return snapshot.RecoverFromSnapshot();
        }

        return null;
    }
}
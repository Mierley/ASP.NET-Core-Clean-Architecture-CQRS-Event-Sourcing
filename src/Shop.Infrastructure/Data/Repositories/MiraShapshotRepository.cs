using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using Shop.Core.SharedKernel;
using Shop.Domain;
using Shop.Domain.Entities.CustomerAggregate;

namespace Shop.Infrastructure.Data.Repositories;

public class MiraSnapshotRepository : IMiraSnapshotRepository
{
    private static readonly IList<Customer> CustomersSnapshots = new List<Customer>();

    public void SaveSnapshotsAsync(IReadOnlyList<BaseEvent> domainEvents)
    {
        var customer = domainEvents.Count == 0
            ? new Customer()
            : GetLastSnapshot(domainEvents.FirstOrDefault()!.AggregateId);
        if (customer == null)
            return;
        customer.Replay(domainEvents);
        CustomersSnapshots.Add(customer);
    }

    public Customer GetLastSnapshot(Guid customerId)
    {
        return CustomersSnapshots.FirstOrDefault(s => s.Id == customerId);
    }
}
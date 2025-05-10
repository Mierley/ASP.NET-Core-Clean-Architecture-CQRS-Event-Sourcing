using System;
using System.Collections.Generic;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.CustomerAggregate;

namespace Shop.Domain;

public interface ISnapshotRepository
{
    void SaveSnapshotsAsync(IReadOnlyList<BaseEvent> domainEvents);
    Customer GetLastSnapshot(Guid customerId);
}
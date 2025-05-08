using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Core.SharedKernel;

namespace Shop.Infrastructure.Data.EventStore;

public interface IEventStore
{
    public Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, string streamName = null);

    public Task<IEnumerable<BaseEvent>> LoadEventsAsync(Guid aggregateId, string streamName = null);
}
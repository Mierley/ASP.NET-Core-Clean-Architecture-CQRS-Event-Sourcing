using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Core.SharedKernel;

public interface IEventStore
{
    public Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, string streamName = null);

    public Task<IEnumerable<BaseEvent>> LoadEventsAsync(Guid aggregateId, string streamName = null);
}
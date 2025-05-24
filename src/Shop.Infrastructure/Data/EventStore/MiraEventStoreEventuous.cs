using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eventuous;
using Eventuous.EventStore.Subscriptions;
using Eventuous.Subscriptions.Checkpoints;
using Eventuous.Subscriptions.Context;
using Shop.Core.SharedKernel;
using StreamSubscription = EventStore.Client.StreamSubscription;
using EventStore.Client;

namespace Shop.Infrastructure.Data.EventStore;

/// <summary>
/// Реализация IMiraEventStore через Eventuous + EventStoreDB
/// </summary>
public class MiraEventStoreEventuous : IMiraEventStore
{
    private readonly IEventStore _store; // EsdbEventStore
    private readonly IEventSerializer _serializer; // дефолтный JSON-серилизатор Eventuous

    public MiraEventStoreEventuous(IEventStore store, IEventSerializer serializer = null)
    {
        _store = store;
        _serializer = serializer ?? DefaultEventSerializer.Instance;
    }

    // ----------------  Save  ----------------
    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, string streamName = null)
    {
        var stream = new StreamName(streamName ?? GetStreamName(aggregateId));

        // ExpectedStreamVersion.Any == «optimistic concurrency отключён»
        await _store.Store(
            stream,
            ExpectedStreamVersion.Any,
            events.Cast<object>().ToArray()); // Eventuous сам завернёт в NewStreamEvent
    }

    // ----------------  Load  ----------------
    public async Task<IEnumerable<BaseEvent>> LoadEventsAsync(Guid aggregateId, long afterVersion = -1,
        string streamName = null)
    {
        var stream = new StreamName(streamName ?? GetStreamName(aggregateId));
        var start = afterVersion < 0
            ? StreamReadPosition.Start
            : new StreamReadPosition(afterVersion + 1);

        // читаем ВСЕ оставшиеся события
        var result = await _store.ReadEvents(stream, start, int.MaxValue, CancellationToken.None);

        // StreamEvent.Event   → уже десериализованное сообщение
        return result.Select(x => (BaseEvent)x.Payload);
    }

    // ----------------  Catch-up subscription  ----------------

    private static string GetStreamName(Guid id) => $"Aggregate-{id}";
}
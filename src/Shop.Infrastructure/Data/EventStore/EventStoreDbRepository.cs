using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.Client;
using Shop.Core.SharedKernel;

namespace Shop.Infrastructure.Data.EventStore;

public class EventStoreDbRepository : IEventStore
{
    private readonly EventStoreClient _client;

    public EventStoreDbRepository(EventStoreClient client)
    {
        _client = client;
    }

    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, string streamName = null)
    {
        string stream = streamName ?? GetStreamName(aggregateId);
        // Подготавливаем данные событий для сохранения
        var eventDataBatch = events.Select(@event =>
        {
            var eventJson = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType());
            var eventType = @event.GetType().Name;
            return new EventData(Uuid.NewUuid(), eventType, eventJson);
        });
        // Запись пачки событий в поток
        await _client.AppendToStreamAsync(
            stream,
            StreamState.Any, // допускаем любую версию потока (optimistic concurrency можно настроить при необходимости)
            eventDataBatch
        );
    }

    /// <summary>
    /// считывает все события потока с начала
    /// </summary>
    /// <param name="aggregateId"></param>
    /// <param name="streamName"></param>
    /// <returns></returns>
    public async Task<IEnumerable<BaseEvent>> LoadEventsAsync(Guid aggregateId, string streamName = null)
    {
        string stream = streamName ?? GetStreamName(aggregateId);
        var result = _client.ReadStreamAsync(Direction.Forwards, stream, StreamPosition.Start);
        var events = new List<BaseEvent>();
        await foreach (var resolvedEvent in result)
        {
            byte[] data = resolvedEvent.Event.Data.ToArray();
            string eventType = resolvedEvent.Event.EventType;
            // Десериализуем данные обратно в объект доменного события
            BaseEvent domainEvent = DeserializeEvent(eventType, data);
            events.Add(domainEvent);
        }

        return events;
    }

    private static string GetStreamName(Guid aggregateId)
    {
        return $"Aggregate-{aggregateId}";
        // Примечание: можно включить в имя потока тип агрегата, например Order-{id}, для уникальности по типам.
    }

    private static BaseEvent DeserializeEvent(string eventType, byte[] data)
    {
        // Пример: по имени типа события восстанавливаем тип .NET
        // Допустим, у нас есть словарь eventTypeName -> Type или переключатель if/else
        // Для простоты: eventType совпадает с именем класса событ
        var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
            .FirstOrDefault(x => x.Name.Contains(eventType));

        if (type == null) throw new InvalidOperationException($"Unknown event type: {eventType}");
        var json = System.Text.Encoding.UTF8.GetString(data);
        return (BaseEvent)JsonSerializer.Deserialize(json, type);
    }
}
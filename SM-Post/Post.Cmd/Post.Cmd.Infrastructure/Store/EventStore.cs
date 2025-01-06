using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Store;

public class EventStore:IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    public EventStore(IEventStoreRepository eventStoreRepository)
    {
        _eventStoreRepository = eventStoreRepository;
    }

    /// <inheritdoc />
    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        if(eventStream.Any() && eventStream.Last().Version != expectedVersion)
        {
            throw new ConcurrencyException($"Incorrect version provided for aggregate: {aggregateId}");
        }
        var version = expectedVersion;

        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel
            {
                TimeStamp = DateTime.Now,
                AggregateId = aggregateId,
                AggregateType = nameof(PostAggregate),
                EventType = eventType,
                EventData = @event,
                Version = version
            };
            await _eventStoreRepository.SaveAsync(eventModel);
        }
    }

    /// <inheritdoc />
    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        if (eventStream == null || !eventStream.Any())
        {
            throw new AggregateNotFoundException($"Incorrect Post Id Provided: {aggregateId}");
        }

        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }
}
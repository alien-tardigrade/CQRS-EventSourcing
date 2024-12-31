using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public interface IEventStoreRepository
{
    // Since an event store is a write-only store, we only need to save events
    // Save an event to the Event Store. The Defined EventModel defines the EventModel that will be stored in the Event Store.
    Task SaveAsync(EventModel @event);

    // We can also retrieve events by the AggregateId. This is the retrieve for all events for a specific Aggregate.
    // This is useful when we need to rebuild the state of an Aggregate.
    Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
}
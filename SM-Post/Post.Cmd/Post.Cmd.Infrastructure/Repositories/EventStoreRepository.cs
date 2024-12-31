using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;

namespace Post.Cmd.Infrastructure.Repositories;

public class EventStoreRepository:IEventStoreRepository
{
    // Define the MongoDB Collection
    private readonly IMongoCollection<EventModel> _eventStoreCollection;

    /* Moved to Singleton
     public EventStoreRepository(IMongoClient mongoClient)
    {
        // Get the database
        var database = mongoClient.GetDatabase("EventStore");
        // Get the collection
        _eventStoreCollection = database.GetCollection<EventModel>("Events");
    }
    */

    // Since the MongoDB connection is managed in Program.cs,
    // the EventStoreRepository class to use dependency injection for the IMongoCollection<EventModel>
    // instead of creating a new MongoClient instance.
    public EventStoreRepository(IMongoCollection<EventModel> eventStoreCollection)
    {
        _eventStoreCollection = eventStoreCollection;
    }

    /// <inheritdoc />
    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStoreCollection.Find(x=>x.AggregateId == aggregateId).ToListAsync().ConfigureAwait(false);
    }
}
using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    protected Guid _id;

    // List to store the Changes. Changes come in the form of events. Events are used to make the changes to the Aggregate.
    private readonly List<BaseEvent> _changes = [];

    public Guid Id => _id;

    public int Version { get; set; } = -1;

    // Method to return the Uncommited changes
    // Returns an enumerable collection of uncommitted changes (events) stored in the _changes list.
    public IEnumerable<BaseEvent> GetUncommittedChanges() => _changes;

    // Method to clear the Uncommited changes (Mark them as Committed)
    public void MarkChangesAsCommitted() => _changes.Clear();

    /**
     * ApplyChange
     *
     * This method is used to apply the Event to the Aggregate.
     * The method gets the method of the Aggregate to apply the Event.
     * The method applies the Event to the Aggregate.
     * The method increments the Version of the Aggregate.
     */
    private void ApplyChange(BaseEvent @event, bool isNew)
    {
        // Apply the Event to the Aggregate. this applies to the type of the concrete aggregate as this is abstract class.
        // We will override this method in the concrete aggregate and there will be a few overloads of this method.
        // Call the "Apply" method of the Aggregate to apply the Event. The event will have the parameters that need to be
        // passed to the Apply method. (REFLECTION Will need to Discover the Apply method)
        var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });
        if (method == null)
        {
            throw new InvalidOperationException($"Method Apply for {@event.GetType().Name} not found.");
        }

        // Invoke the Method
        method.Invoke(this, new object[] { @event });

        // This is a new Event so add it to the list of Uncommitted Changes
        // This will ensure we do not write the same event to the Event Store multiple times.
        if (isNew)
        {
            _changes.Add(@event);
        }

        // Increment the Version of the Aggregate
        Version++;
    }

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChange(@event, true);
    }

    /**
     * ReplayEvents
     *
     * This method is used to replay the Events.
     * The method replays the Events to the Aggregate. This is used to rebuild the Aggregate.
     */
    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var @event in events)
        {
            // Ensure the isNew flag is set to false as we are replaying the events and these are not new Events
            ApplyChange(@event, false);
        }
    }
}
using CQRS.Core.Messages;

namespace CQRS.Core.Events;

/**
 * BaseEvent class from which all other events will inherit.
 */
public abstract class BaseEvent:Message
{

    public string EventType { get; set; }
    public string Version { get; set; }

    // Set the event type via the constructor
    protected BaseEvent(string eventType)
    {
        this.EventType = eventType;
    }
}
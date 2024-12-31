namespace CQRS.Core.Messages;

/**
 * This Project provides the framework classes for the CQRS pattern.
 * Base class for all messages.
 */
public abstract class Message
{
    public Guid Id { get; set; }
}
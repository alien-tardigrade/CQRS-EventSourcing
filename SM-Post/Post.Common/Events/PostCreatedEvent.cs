using CQRS.Core.Events;

namespace Post.Common.Events;

public class PostCreatedEvent:BaseEvent
{
    /// <inheritdoc />
    /// Passing the event type to the base class
    public PostCreatedEvent() : base(nameof(PostCreatedEvent))
    {
    }

    public string Author { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public DateTime DatePosted { get; set; }
}
using CQRS.Core.Events;

namespace Post.Common.Events;

public class PostUpdatedEvent:BaseEvent
{
    /// <inheritdoc />
    public PostUpdatedEvent() : base(nameof(PostUpdatedEvent))
    {
    }

    public string Title { get; set; }
    public string Content { get; set; }
}
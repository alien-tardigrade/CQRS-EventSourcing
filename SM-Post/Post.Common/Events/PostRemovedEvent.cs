using CQRS.Core.Events;

namespace Post.Common.Events;

public class PostRemovedEvent:BaseEvent
{
    /// <inheritdoc />
    public PostRemovedEvent() : base(nameof(PostRemovedEvent))
    {
    }
}
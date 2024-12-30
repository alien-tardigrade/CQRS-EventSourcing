using CQRS.Core.Events;

namespace Post.Common.Events;

public class PostLikedEvent:BaseEvent
{
    /// <inheritdoc />
    public PostLikedEvent() : base(nameof(PostLikedEvent))
    {
    }

}
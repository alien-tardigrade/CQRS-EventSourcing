using CQRS.Core.Events;

namespace Post.Common.Events;

public class CommentUpdatedEvent:BaseEvent
{
    /// <inheritdoc />
    public CommentUpdatedEvent() : base(nameof(CommentUpdatedEvent))
    {
    }

    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public string UserName { get; set; }
    public DateTime EditDate { get; set; }
}
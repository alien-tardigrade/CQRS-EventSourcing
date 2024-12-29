using CQRS.Core.Events;

namespace Post.Common.Events;

public class CommentAddedEvent:BaseEvent
{
    /// <inheritdoc />
    public CommentAddedEvent() : base(nameof(CommentAddedEvent))
    {
    }

    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public string UserName { get; set; }
    public DateTime CommentDate { get; set; }
}
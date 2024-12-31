using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate: AggregateRoot
{
    private bool _active;
    private string _title;
    private string _content;
    private string _author;

    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    public PostAggregate()
    {

    }

    /**
     * PostAggregate
     *
     * This constructor is used to create the Post Aggregate.
     * The constructor raises the PostCreatedEvent Event.
     */
    public PostAggregate(Guid id, string title, string content, string author)
    {
        // So basically this is how the flow will work for this
        // 1. The RaiseEvent method will be called (See AggregateRoot in abstract class)
        // 2. The ApplyChange method will be called (See AggregateRoot in abstract class)
        // 3. The ApplyChange method will call the Apply method of the Aggregate (This class) and pass the Event to it
        // 4. The Apply method will apply the Event to the Aggregate
        // 5. Since it's a new Event, in the ApplyChange method, the Event will be added to the list of Uncommitted Changes
        // 6. The Version of the Aggregate will be incremented
        RaiseEvent(new PostCreatedEvent()
        {
            Id = id,
            Author = author,
            Content = content,
            Title = title,
            DatePosted = DateTime.Now
        });
    }

    // Concrete Apply method and get the Parameters from the Event and alter the state of the Aggregate
    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _author = @event.Author;
        _content = @event.Content;
        _title = @event.Title;
        _active = true;
    }

    // Method to Edit the Post
    public void Edit(string title, string content)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Post is not Active and you cannot edit an Inactive Post.");
        }

        if (title == null || content == null)
        {
            throw new InvalidOperationException($"{nameof(title)} and {nameof(content)} cannot be null. " +
                                                $"Please provide a valid {nameof(title)} and {nameof(content)}.");
        }

        RaiseEvent(new PostUpdatedEvent()
        {
            Id = _id,
            Title = title,
            Content = content
        });
    }

    /**
     * Apply for the Updated Post Event
     */
    public void Apply(PostUpdatedEvent @event)
    {
        _id = @event.Id;
    }

    // Like Post
    public void LikePost()
    {
        if (!_active)
        {
            throw new InvalidOperationException("Post is not Active and you cannot Like an Inactive Post.");
        }
        RaiseEvent(new PostLikedEvent()
        {
            Id = _id
        });
    }

    /**
     * Apply for the Post Liked Event
     */
    public void Apply(PostLikedEvent @event)
    {
        _id = @event.Id;
    }

    // Add Comment
    public void AddComment(  string comment, string userName)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Post is not Active and you cannot add a Comment to an Inactive Post.");
        }

        if (comment == null || userName == null)
        {
            throw new InvalidOperationException($"{nameof(comment)} and {nameof(userName)} cannot be null. " +
                                                $"Please provide a valid {nameof(comment)} and {nameof(userName)}.");
        }

        RaiseEvent(new CommentAddedEvent()
        {
            Id = _id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            UserName = userName,
            CommentDate = DateTime.Now
        });
    }

    /**
     * Apply for the Comment Added Event
     */
    public void Apply(CommentAddedEvent @event)
    {
        _id = @event.Id;
        _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.UserName));
    }

    // Edit Comment
    public  void EditComment(Guid commentId, string comment, string userName)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Post is not Active and you cannot edit a Comment of an Inactive Post.");
        }

        if (comment == null)
        {
            throw new InvalidOperationException($"{nameof(comment)} cannot be null. " +
                                                $"Please provide a valid {nameof(comment)}.");
        }

        if (!_comments.ContainsKey(commentId))
        {
            throw new InvalidOperationException($"Comment with Id {commentId} not found.");
        }

        if (!_comments[commentId].Item2.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException($"User {userName} is not authorized to edit the Comment as the you are not the user that made the comment.");
        }

        RaiseEvent(new CommentUpdatedEvent()
        {
            Id = _id,
            CommentId = commentId,
            Comment = comment,
            UserName =  userName,
            EditDate = DateTime.Now
        });
    }

    /**
     * Apply for the Comment Updated Event
     */
    public void Apply(CommentUpdatedEvent @event)
    {
        _id = @event.Id;
        _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.UserName);
    }

    // Delete Comment
    public void DeleteComment(Guid commentId, string userName)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Post is not Active and you cannot delete a Comment of an Inactive Post.");
        }

        if (!_comments.ContainsKey(commentId))
        {
            throw new InvalidOperationException($"Comment with Id {commentId} not found.");
        }

        if (!_comments[commentId].Item2.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException($"User {userName} is not authorized to delete the Comment as the you are not the user that made the comment.");
        }

        RaiseEvent(new CommentRemovedEvent()
        {
            Id = _id,
            CommentId = commentId
        });
    }

    /**
     * Apply for the Comment Removed Event
     */
    public void Apply(CommentRemovedEvent @event)
    {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    // Remove Post
    public void RemovePost(string author)
    {
        if (!_active)
        {
            throw new InvalidOperationException("Post is not Active and you cannot remove an Inactive Post.");
        }

        if (!_author.Equals(author, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException($"User {author} is not authorized to remove the Post as the you are not the user that made the post.");
        }

        RaiseEvent(new PostRemovedEvent()
        {
            Id = _id
        });
    }

    /**
     * Apply for the Post Removed Event
     */
    public void Apply(PostRemovedEvent @event)
    {
        _id = @event.Id;
        _active = false;
    }
}
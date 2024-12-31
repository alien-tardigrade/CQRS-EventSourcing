using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class EditPostCommand:BaseCommand
{
    public string Title { get; set; }
    public string Content { get; set; }
}
using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class NewPostCommand:BaseCommand
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
}

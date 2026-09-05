namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;

internal class Post(int authorId, string content)
{
    public int Id { get; private set; }

    public int AuthorId { get; private set; } = authorId;

    public string Content { get; private set; } = content;

    public PostStatus Status { get; set; } = PostStatus.Published;

    public enum PostStatus
    {
        Published,
        Unpublished
    }
}

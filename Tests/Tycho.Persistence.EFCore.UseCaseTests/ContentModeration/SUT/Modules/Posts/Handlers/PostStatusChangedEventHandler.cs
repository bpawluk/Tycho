using Tycho.Events;
using Tycho.Transactions;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class PostStatusChangedEventHandler(PostsDbContext dbContext) : ITransactionalEventHandler<PostStatusChangedEvent>
{
    public async Task HandleAsync(EventContext<PostStatusChangedEvent> context, CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.FindAsync([context.Payload.PostId], cancellationToken);
        if (post is null)
        {
            throw new ArgumentException($"There is no Posts with ID {context.Payload.PostId}");
        }
        post.Status = GetStatus(context.Payload.NewStatus);
    }

    private static Post.PostStatus GetStatus(PostStatusChangedEvent.Status status)
    {
        return status switch
        {
            PostStatusChangedEvent.Status.Published => Post.PostStatus.Published,
            PostStatusChangedEvent.Status.Unpublished => Post.PostStatus.Unpublished,
            _ => throw new ArgumentException($"Unknown status {status}", nameof(status))
        };
    }
}

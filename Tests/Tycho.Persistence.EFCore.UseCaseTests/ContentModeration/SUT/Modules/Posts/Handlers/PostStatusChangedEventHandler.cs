using Tycho.Events;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Persistence;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class PostStatusChangedEventHandler(PostsDbContext dbContext) : ITransactionalEventHandler<PostStatusChangedEvent>
{
    public async Task HandleAsync(EventContext<PostStatusChangedEvent> context, CancellationToken cancellationToken)
    {
        Post? post = await dbContext.Posts.FindAsync([context.Payload.PostId], cancellationToken) ?? throw new ArgumentException($"There is no Posts with ID {context.Payload.PostId}");
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

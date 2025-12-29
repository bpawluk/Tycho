using Tycho.Events;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class PostStatusChangedEventHandler(IUnitOfWork unitOfWork) : IEventHandler<PostStatusChangedEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(PostStatusChangedEvent eventData, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken); // Simulate async work
        var posts = _unitOfWork.Set<Post>();

        var post = await posts.FindAsync([eventData.PostId], cancellationToken);
        if (post is null)
        {
            throw new ArgumentException($"There is no Posts with ID {eventData.PostId}");
        }

        post.Status = GetStatus(eventData.NewStatus);
        await _unitOfWork.SaveChanges(cancellationToken);
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
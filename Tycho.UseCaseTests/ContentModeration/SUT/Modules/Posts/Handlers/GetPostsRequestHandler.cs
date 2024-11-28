using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class GetPostsRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPostsRequest, GetPostsRequest.Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<GetPostsRequest.Response> Handle(GetPostsRequest requestData, CancellationToken cancellationToken)
    {
        var posts = _unitOfWork.Set<Post>();
        var responsePosts = await posts
            .Where(post => post.Status == Post.PostStatus.Published)
            .Select(post => new GetPostsRequest.Post(
                post.Id,
                post.AuthorId,
                post.Content))
            .ToArrayAsync(cancellationToken);
        return new GetPostsRequest.Response(responsePosts);
    }
}
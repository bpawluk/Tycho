using Microsoft.EntityFrameworkCore;
using Tycho.Requests;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class GetPostsRequestHandler(PostsDbContext dbContext) : IRequestHandler<GetPostsRequest, GetPostsRequest.Response>
{
    public async Task<GetPostsRequest.Response> HandleAsync(GetPostsRequest requestData, CancellationToken cancellationToken)
    {
        var responsePosts = await dbContext.Posts
            .Where(post => post.Status == Post.PostStatus.Published)
            .Select(post => new GetPostsRequest.Post(
                post.Id,
                post.AuthorId,
                post.Content))
            .ToArrayAsync(cancellationToken);
        return new GetPostsRequest.Response(responsePosts);
    }
}
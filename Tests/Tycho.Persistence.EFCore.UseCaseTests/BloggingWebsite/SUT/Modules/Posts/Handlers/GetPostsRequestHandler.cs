using Microsoft.EntityFrameworkCore;
using Tycho.Requests;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Persistence;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract.GetPostsRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Handlers;

internal class GetPostsRequestHandler(PostsDbContext dbContext) : IRequestHandler<GetPostsRequest, Response>
{

    public async Task<Response> HandleAsync(GetPostsRequest requestData, CancellationToken cancellationToken)
    {
        var responsePosts = await dbContext.Posts
            .Where(post => requestData.PostIds.Contains(post.Id))
            .Select(post => new Post(
                post.Id,
                post.Author,
                post.Content))
            .ToArrayAsync(cancellationToken);
        return new Response(responsePosts);
    }
}
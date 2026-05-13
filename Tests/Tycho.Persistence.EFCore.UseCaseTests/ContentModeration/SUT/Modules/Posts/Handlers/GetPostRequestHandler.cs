using Tycho.Requests;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class GetPostRequestHandler(PostsDbContext dbContext) : IRequestHandler<GetPostRequest, GetPostRequest.Response>
{
    public async Task<GetPostRequest.Response> HandleAsync(GetPostRequest requestData, CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.FindAsync([requestData.PostId], cancellationToken);
        if (post is null)
        {
            throw new ArgumentException($"There is no Posts with ID {requestData.PostId}");
        }
        return new GetPostRequest.Response(new(post.Id, post.AuthorId, post.Content));
    }
}
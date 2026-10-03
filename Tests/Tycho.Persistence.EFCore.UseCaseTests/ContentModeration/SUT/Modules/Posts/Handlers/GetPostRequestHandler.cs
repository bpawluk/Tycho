using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Persistence;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class GetPostRequestHandler(PostsDbContext dbContext) : IRequestHandler<GetPostRequest, GetPostRequest.Response>
{
    public async Task<GetPostRequest.Response> HandleAsync(GetPostRequest requestData, CancellationToken cancellationToken)
    {
        Post? post = await dbContext.Posts.FindAsync([requestData.PostId], cancellationToken) ?? throw new ArgumentException($"There is no Posts with ID {requestData.PostId}");
        return new GetPostRequest.Response(new(post.Id, post.AuthorId, post.Content));
    }
}

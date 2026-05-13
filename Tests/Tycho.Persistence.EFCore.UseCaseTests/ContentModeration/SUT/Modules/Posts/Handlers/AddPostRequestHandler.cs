using Tycho.Requests;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class AddPostRequestHandler(PostsDbContext dbContext) : IRequestHandler<AddPostRequest, AddPostRequest.Response>
{
    public async Task<AddPostRequest.Response> HandleAsync(AddPostRequest requestData, CancellationToken cancellationToken)
    {
        var newPost = new Post(requestData.AuthorId, requestData.Content);
        dbContext.Posts.Add(newPost);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new(newPost.Id);
    }
}
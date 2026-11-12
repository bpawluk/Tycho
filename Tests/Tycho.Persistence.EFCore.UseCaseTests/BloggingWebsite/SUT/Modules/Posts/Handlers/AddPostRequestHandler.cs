using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Persistence;
using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract.AddPostRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Handlers;

internal class AddPostRequestHandler(PostsDbContext dbContext) : IRequestHandler<AddPostRequest, Response>
{

    public async Task<Response> HandleAsync(AddPostRequest requestData, CancellationToken cancellationToken)
    {
        var newPost = new Post(requestData.Author, requestData.Content);
        dbContext.Posts.Add(newPost);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new Response(newPost.Id);
    }
}

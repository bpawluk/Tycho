using Tycho.Requests;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Domain;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Persistence;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract.AddPostRequest;

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
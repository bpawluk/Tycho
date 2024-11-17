using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract.GetPostsRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Handlers;

internal class GetPostsRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPostsRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response> Handle(GetPostsRequest requestData, CancellationToken cancellationToken)
    {
        var posts = _unitOfWork.Set<Domain.Post>();
        var responsePosts = await posts
            .Where(post => requestData.PostIds.Contains(post.Id))
            .Select(post => new Post(
                post.Id,
                post.Author,
                post.Content))
            .ToArrayAsync(cancellationToken);
        return new Response(responsePosts);
    }
}
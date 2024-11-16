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
        var articles = _unitOfWork.Set<Domain.Post>();
        var responsePosts = await articles
            .Where(article => requestData.PostIds.Contains(article.Id))
            .Select(article => new Post(
                article.Id,
                article.Author,
                article.Content))
            .ToArrayAsync(cancellationToken);
        return new Response(responsePosts);
    }
}
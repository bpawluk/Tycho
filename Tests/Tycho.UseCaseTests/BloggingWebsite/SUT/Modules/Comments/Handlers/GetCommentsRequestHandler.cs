using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract.GetCommentsRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Handlers;

internal class GetCommentsRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCommentsRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response> Handle(GetCommentsRequest requestData, CancellationToken cancellationToken)
    {
        var posts = _unitOfWork.Set<Domain.Comment>();
        var responseComments = await posts
            .Where(post => requestData.CommentIds.Contains(post.Id))
            .Select(post => new Comment(
                post.Id,
                post.Author,
                post.Content))
            .ToArrayAsync(cancellationToken);
        return new Response(responseComments);
    }
}
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
        var articles = _unitOfWork.Set<Domain.Comment>();
        var responseComments = await articles
            .Where(article => requestData.CommentIds.Contains(article.Id))
            .Select(article => new Comment(
                article.Id,
                article.Author,
                article.Content))
            .ToArrayAsync(cancellationToken);
        return new Response(responseComments);
    }
}
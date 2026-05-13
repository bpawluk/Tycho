using Microsoft.EntityFrameworkCore;
using Tycho.Requests;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Persistence;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract.GetCommentsRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Handlers;

internal class GetCommentsRequestHandler(CommentsDbContext dbContext) : IRequestHandler<GetCommentsRequest, Response>
{

    public async Task<Response> HandleAsync(GetCommentsRequest requestData, CancellationToken cancellationToken)
    {
        var responseComments = await dbContext.Comments
            .Where(post => requestData.CommentIds.Contains(post.Id))
            .Select(post => new Comment(
                post.Id,
                post.Author,
                post.Content))
            .ToArrayAsync(cancellationToken);
        return new Response(responseComments);
    }
}
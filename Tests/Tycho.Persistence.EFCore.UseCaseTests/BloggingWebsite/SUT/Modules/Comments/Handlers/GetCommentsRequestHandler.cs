using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Persistence;
using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract.GetCommentsRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Handlers;

internal class GetCommentsRequestHandler(CommentsDbContext dbContext) : IRequestHandler<GetCommentsRequest, Response>
{

    public async Task<Response> HandleAsync(GetCommentsRequest requestData, CancellationToken cancellationToken)
    {
        Comment[] responseComments = await dbContext.Comments
            .Where(post => requestData.CommentIds.Contains(post.Id))
            .Select(post => new Comment(
                post.Id,
                post.Author,
                post.Content))
            .ToArrayAsync(cancellationToken);
        return new Response(responseComments);
    }
}

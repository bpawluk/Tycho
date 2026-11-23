using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Persistence;
using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract.AddCommentRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Handlers;

internal class AddCommentRequestHandler(CommentsDbContext dbContext) : IRequestHandler<AddCommentRequest, Response>
{

    public async Task<Response> HandleAsync(AddCommentRequest requestData, CancellationToken cancellationToken)
    {
        var newComment = new Comment(requestData.Author, requestData.Content);
        dbContext.Comments.Add(newComment);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new Response(newComment.Id);
    }
}

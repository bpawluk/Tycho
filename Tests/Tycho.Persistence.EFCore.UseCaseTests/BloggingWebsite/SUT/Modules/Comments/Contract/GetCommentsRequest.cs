using Tycho.Requests;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract.GetCommentsRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract;

public record GetCommentsRequest(IReadOnlyList<int> CommentIds) : IRequest<Response>
{
    public record Response(IReadOnlyList<Comment> Comments);

    public record Comment(int Id, string Author, string Content);
}
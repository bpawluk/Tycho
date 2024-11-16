using Tycho.Requests;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract.AddCommentRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract;

public record AddCommentRequest(string Author, string Content) : IRequest<Response>
{
    public record Response(int CommentId);
}
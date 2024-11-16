using Tycho.Requests;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract.AddPostRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract;

public record AddPostRequest(string Author, string Content) : IRequest<Response>
{
    public record Response(int PostId);
}
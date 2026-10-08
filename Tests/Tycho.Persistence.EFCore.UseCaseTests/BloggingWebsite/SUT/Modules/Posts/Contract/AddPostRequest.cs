using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract.AddPostRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract;

public record AddPostRequest(string Author, string Content) : IRequest<Response>
{
    public record Response(int PostId);
}

using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract.GetPostsRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract;

public record GetPostsRequest(IReadOnlyList<int> PostIds) : IRequest<Response>
{
    public record Response(IReadOnlyList<Post> Posts);

    public record Post(int Id, string Author, string Content);
}

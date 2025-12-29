using Tycho.Requests;
using static Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract.GetPostsRequest;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;

public record GetPostsRequest : IRequest<Response>
{
    public record Response(IReadOnlyList<Post> Posts);

    public record Post(int Id, int AuthorId, string Content);
}
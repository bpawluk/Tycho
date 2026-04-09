using Tycho.Requests;
using static Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract.GetPostRequest;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;

public record GetPostRequest(int PostId) : IRequest<Response>
{
    public record Response(Post Post);

    public record Post(int Id, int AuthorId, string Content);
}
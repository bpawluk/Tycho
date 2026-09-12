using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract.AddPostRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;

public record AddPostRequest(int AuthorId, string Content) : IRequest<Response>
{
    public record Response(int PostId);
}

using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;

public record GetAuditedPostsRequest : IRequest<GetAuditedPostsRequest.Response>
{
    public record Response(IReadOnlyList<int> PostIds);
}

using Microsoft.EntityFrameworkCore;
using Tycho.Requests;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class GetAuditedPostsRequestHandler(PostsDbContext dbContext)
    : IRequestHandler<GetAuditedPostsRequest, GetAuditedPostsRequest.Response>
{
    public async Task<GetAuditedPostsRequest.Response> HandleAsync(GetAuditedPostsRequest requestData, CancellationToken cancellationToken)
    {
        int[] ids = await dbContext.Posts
            .Where(subject => subject.AuditId != null)
            .Select(subject => subject.Id)
            .ToArrayAsync(cancellationToken);
        return new(ids);
    }
}

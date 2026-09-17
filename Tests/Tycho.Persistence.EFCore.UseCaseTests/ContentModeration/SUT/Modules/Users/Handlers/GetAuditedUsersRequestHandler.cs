using Microsoft.EntityFrameworkCore;
using Tycho.Requests;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Handlers;

internal class GetAuditedUsersRequestHandler(UsersDbContext dbContext)
    : IRequestHandler<GetAuditedUsersRequest, GetAuditedUsersRequest.Response>
{
    public async Task<GetAuditedUsersRequest.Response> HandleAsync(GetAuditedUsersRequest requestData, CancellationToken cancellationToken)
    {
        int[] ids = await dbContext.Users
            .Where(subject => subject.AuditId != null)
            .Select(subject => subject.Id)
            .ToArrayAsync(cancellationToken);
        return new(ids);
    }
}

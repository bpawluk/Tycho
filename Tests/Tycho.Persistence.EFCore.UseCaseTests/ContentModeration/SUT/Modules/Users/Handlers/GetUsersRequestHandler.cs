using Microsoft.EntityFrameworkCore;
using Tycho.Requests;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Handlers;

internal class GetUsersRequestHandler(UsersDbContext dbContext) : IRequestHandler<GetUsersRequest, GetUsersRequest.Response>
{
    public async Task<GetUsersRequest.Response> HandleAsync(GetUsersRequest requestData, CancellationToken cancellationToken)
    {
        var responseUsers = await dbContext.Users
            .Where(user => user.Status == User.UserStatus.Active)
            .Select(user => new GetUsersRequest.User(user.Id, user.Name))
            .ToArrayAsync(cancellationToken);
        return new GetUsersRequest.Response(responseUsers);
    }
}

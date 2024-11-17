using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Domain;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Handlers;

internal class GetUsersRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUsersRequest, GetUsersRequest.Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<GetUsersRequest.Response> Handle(GetUsersRequest requestData, CancellationToken cancellationToken)
    {
        var users = _unitOfWork.Set<User>();
        var responseUsers = await users
            .Where(user => user.Status == User.UserStatus.Active)
            .Select(user => new GetUsersRequest.User(
                user.Id,
                user.Name))
            .ToArrayAsync(cancellationToken);
        return new GetUsersRequest.Response(responseUsers);
    }
}
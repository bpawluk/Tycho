using Tycho.Requests;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Domain;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Handlers;

internal class AddUserRequestHandler(UsersDbContext dbContext) : IRequestHandler<AddUserRequest, AddUserRequest.Response>
{
    public async Task<AddUserRequest.Response> HandleAsync(AddUserRequest requestData, CancellationToken cancellationToken)
    {
        var newUser = new User(requestData.Name);
        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new(newUser.Id);
    }
}
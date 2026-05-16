using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract.AddUserRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

public record AddUserRequest(string Name) : IRequest<Response>
{
    public record Response(int UserId);
}

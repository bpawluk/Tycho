using Tycho.Requests;
using static Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract.AddUserRequest;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

public record AddUserRequest(string Name) : IRequest<Response>
{
    public record Response(int UserId);
}
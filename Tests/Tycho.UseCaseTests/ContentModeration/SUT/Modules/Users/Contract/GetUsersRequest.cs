using Tycho.Requests;
using static Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract.GetUsersRequest;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

public record GetUsersRequest : IRequest<Response>
{
    public record Response(IReadOnlyList<User> Users);

    public record User(int Id, string Name);
}
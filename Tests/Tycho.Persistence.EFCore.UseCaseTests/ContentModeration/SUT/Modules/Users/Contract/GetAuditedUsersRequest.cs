using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

public record GetAuditedUsersRequest : IRequest<GetAuditedUsersRequest.Response>
{
    public record Response(IReadOnlyList<int> UserIds);
}

using Tycho.Requests;
using static Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing.GetAuthorRequest;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;

public record GetAuthorRequest(int PostId) : IRequest<Response>
{
    public record Response(int AuthorId);
}
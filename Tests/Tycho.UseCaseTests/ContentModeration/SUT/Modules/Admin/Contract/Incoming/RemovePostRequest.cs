using Tycho.Requests;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Incoming;

public record RemovePostRequest(int PostId, bool BanAuthor = false) : IRequest;
using Tycho.Requests;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Incoming;

public record AddReactionRequest(int TargetId) : IRequest;
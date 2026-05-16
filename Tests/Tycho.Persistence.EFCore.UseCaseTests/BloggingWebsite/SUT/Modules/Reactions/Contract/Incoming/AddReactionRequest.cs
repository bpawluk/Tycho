using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Incoming;

public record AddReactionRequest(int TargetId) : IRequest;

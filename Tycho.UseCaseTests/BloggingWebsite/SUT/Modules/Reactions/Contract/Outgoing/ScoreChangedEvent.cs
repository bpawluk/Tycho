using Tycho.Events;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Outgoing;

public record ScoreChangedEvent(int TargetId, uint NewScore) : IEvent;
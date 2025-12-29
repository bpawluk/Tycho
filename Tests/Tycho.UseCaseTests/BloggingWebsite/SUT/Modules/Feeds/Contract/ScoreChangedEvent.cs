using Tycho.Events;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;

public record ScoreChangedEvent(int EntryId, uint NewScore) : IEvent;
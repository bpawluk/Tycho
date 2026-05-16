using Tycho.Events;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;

public record ScoreChangedEvent(int EntryId, uint NewScore) : IEvent;

using Tycho.Events;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;

public record UserBannedEvent(int UserId) : IEvent;
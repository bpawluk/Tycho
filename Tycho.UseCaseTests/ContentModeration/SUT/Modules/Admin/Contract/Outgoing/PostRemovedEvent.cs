using Tycho.Events;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;

public record PostRemovedEvent(int PostId) : IEvent;
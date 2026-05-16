using Tycho.Events;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;

public record PostRemovedEvent(int PostId) : IEvent;

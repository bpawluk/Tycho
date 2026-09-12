using Tycho.Events;
using static Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract.UserStatusChangedEvent;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

public record UserStatusChangedEvent(int UserId, Status NewStatus) : IEvent
{
    public enum Status
    {
        Active,
        Deactivated
    }
}

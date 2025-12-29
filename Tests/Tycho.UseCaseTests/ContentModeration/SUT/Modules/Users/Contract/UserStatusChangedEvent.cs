using Tycho.Events;
using static Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract.UserStatusChangedEvent;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

public record UserStatusChangedEvent(int UserId, Status NewStatus) : IEvent
{
    public enum Status
    {
        Active,
        Deactivated
    }
}
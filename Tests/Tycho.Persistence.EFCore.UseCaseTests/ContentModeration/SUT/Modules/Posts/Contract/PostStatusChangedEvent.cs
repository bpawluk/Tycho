using Tycho.Events;
using static Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract.PostStatusChangedEvent;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;

public record PostStatusChangedEvent(int PostId, Status NewStatus) : IEvent
{
    public enum Status
    {
        Published,
        Unpublished
    }
}

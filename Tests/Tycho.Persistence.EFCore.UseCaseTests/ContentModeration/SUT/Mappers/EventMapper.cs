using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Mappers;

internal static class EventMapper
{
    public static PostStatusChangedEvent Map(PostRemovedEvent eventData)
    {
        return new(eventData.PostId, PostStatusChangedEvent.Status.Unpublished);
    }

    public static UserStatusChangedEvent Map(UserBannedEvent eventData)
    {
        return new(eventData.UserId, UserStatusChangedEvent.Status.Deactivated);
    }
}

using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Mappers;

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
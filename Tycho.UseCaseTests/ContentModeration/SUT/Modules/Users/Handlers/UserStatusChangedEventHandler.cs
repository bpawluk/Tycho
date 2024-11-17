using Tycho.Events;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Domain;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Handlers;

internal class UserStatusChangedEventHandler(IUnitOfWork unitOfWork) : IEventHandler<UserStatusChangedEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(UserStatusChangedEvent eventData, CancellationToken cancellationToken)
    {
        var users = _unitOfWork.Set<User>();

        var user = await users.FindAsync([eventData.UserId], cancellationToken);
        if (user is null)
        {
            throw new ArgumentException($"There is no Users with ID {eventData.UserId}");
        }

        user.Status = GetStatus(eventData.NewStatus);
        await _unitOfWork.SaveChanges(cancellationToken);
    }

    private static User.UserStatus GetStatus(UserStatusChangedEvent.Status status)
    {
        return status switch
        {
            UserStatusChangedEvent.Status.Active => User.UserStatus.Active,
            UserStatusChangedEvent.Status.Deactivated => User.UserStatus.Deactivated,
            _ => throw new ArgumentException($"Unknown status {status}", nameof(status))
        };
    }
}
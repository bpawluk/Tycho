using Tycho.Events;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Domain;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Handlers;

internal class UserStatusChangedEventHandler(IUnitOfWork unitOfWork) : IEventHandler<UserStatusChangedEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task HandleAsync(EventContext<UserStatusChangedEvent> context, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken); // Simulate async work
        var users = _unitOfWork.Set<User>();

        var user = await users.FindAsync([context.Payload.UserId], cancellationToken);
        if (user is null)
        {
            throw new ArgumentException($"There is no Users with ID {context.Payload.UserId}");
        }

        user.Status = GetStatus(context.Payload.NewStatus);
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
using Tycho.Events;
using Tycho.Transactions;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Handlers;

internal class UserStatusChangedEventHandler(UsersDbContext dbContext) : ITransactionalEventHandler<UserStatusChangedEvent>
{
    public async Task HandleAsync(EventContext<UserStatusChangedEvent> context, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindAsync([context.Payload.UserId], cancellationToken);
        if (user is null)
        {
            throw new ArgumentException($"There is no Users with ID {context.Payload.UserId}");
        }
        user.Status = GetStatus(context.Payload.NewStatus);
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

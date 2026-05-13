using Tycho.Transactions;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Incoming;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Domain;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Persistence;
using static Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.AdminModule;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Handlers;

internal class RemovePostRequestHandler(AdminDbContext dbContext, IParent parent, IPublisher publisher) : ITransactionalRequestHandler<RemovePostRequest>
{
    public async Task HandleAsync(RemovePostRequest requestData, CancellationToken cancellationToken)
    {
        AdminAction newAdminAction;
        if (requestData.BanAuthor)
        {
            var author = await parent.ExecuteAsync(new GetAuthorRequest(requestData.PostId), cancellationToken);
            newAdminAction = AdminAction.RemovePostAndBanAuthor(requestData.PostId, author.AuthorId);
            await publisher.PublishAsync(new UserBannedEvent(author.AuthorId), cancellationToken);
        }
        else
        {
            newAdminAction = AdminAction.RemovePost(requestData.PostId);
        }
        dbContext.AdminActions.Add(newAdminAction);
        await publisher.PublishAsync(new PostRemovedEvent(requestData.PostId), cancellationToken);
    }
}
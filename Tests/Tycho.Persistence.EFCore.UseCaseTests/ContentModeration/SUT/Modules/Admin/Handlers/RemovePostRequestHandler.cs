using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Persistence;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Handlers;

internal class RemovePostRequestHandler(AdminDbContext dbContext, AdminModule.IParent parent, IAdminModulePublisher publisher) : ITransactionalRequestHandler<RemovePostRequest>
{
    public async Task HandleAsync(RemovePostRequest requestData, CancellationToken cancellationToken)
    {
        AdminAction newAdminAction;
        if (requestData.BanAuthor)
        {
            GetAuthorRequest.Response author = await parent.ExecuteAsync(new GetAuthorRequest(requestData.PostId), cancellationToken);
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

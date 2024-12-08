using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.Structure;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Incoming;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Domain;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Handlers;

internal class RemovePostRequestHandler(IUnitOfWork unitOfWork, IParent parent) : IRequestHandler<RemovePostRequest>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IParent _parent = parent;

    public async Task Handle(RemovePostRequest requestData, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken); // Simulate async work

        var adminActions = _unitOfWork.Set<AdminAction>();

        AdminAction newAdminAction;
        if (requestData.BanAuthor)
        {
            var author = await _parent.Execute<GetAuthorRequest, GetAuthorRequest.Response>(new(requestData.PostId), cancellationToken);
            newAdminAction = AdminAction.RemovePostAndBanAuthor(requestData.PostId, author.AuthorId);
            await _unitOfWork.Publish(new UserBannedEvent(author.AuthorId), cancellationToken);
        }
        else
        {
            newAdminAction = AdminAction.RemovePost(requestData.PostId);
        }
        adminActions.Add(newAdminAction);

        await _unitOfWork.Publish(new PostRemovedEvent(requestData.PostId), cancellationToken);
        await _unitOfWork.SaveChanges(cancellationToken);
    }
}
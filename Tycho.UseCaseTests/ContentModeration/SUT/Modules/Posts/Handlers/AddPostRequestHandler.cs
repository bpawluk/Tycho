using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class AddPostRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddPostRequest, AddPostRequest.Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AddPostRequest.Response> Handle(AddPostRequest requestData, CancellationToken cancellationToken)
    {
        var posts = _unitOfWork.Set<Post>();
        var newPost = new Post(requestData.AuthorId, requestData.Content);
        posts.Add(newPost);
        await _unitOfWork.SaveChanges(cancellationToken);
        return new(newPost.Id);
    }
}
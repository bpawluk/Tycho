using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;

internal class GetPostRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPostRequest, GetPostRequest.Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<GetPostRequest.Response> Handle(GetPostRequest requestData, CancellationToken cancellationToken)
    {
        var posts = _unitOfWork.Set<Post>();

        var post = await posts.FindAsync([requestData.PostId], cancellationToken);
        if (post is null)
        {
            throw new ArgumentException($"There is no Posts with ID {requestData.PostId}");
        }

        return new GetPostRequest.Response(new(post.Id, post.AuthorId, post.Content));
    }
}
using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Domain;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract.AddPostRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Handlers;

internal class AddPostRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddPostRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response> Handle(AddPostRequest requestData, CancellationToken cancellationToken)
    {
        var articles = _unitOfWork.Set<Post>();
        var newPost = new Post(requestData.Author, requestData.Content);
        articles.Add(newPost);
        await _unitOfWork.SaveChanges(cancellationToken);
        return new Response(newPost.Id);
    }
}
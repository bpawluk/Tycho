using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Domain;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract.AddCommentRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Handlers;

internal class AddCommentRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddCommentRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response> Handle(AddCommentRequest requestData, CancellationToken cancellationToken)
    {
        var articles = _unitOfWork.Set<Comment>();
        var newComment = new Comment(requestData.Author, requestData.Content);
        articles.Add(newComment);
        await _unitOfWork.SaveChanges(cancellationToken);
        return new Response(newComment.Id);
    }
}
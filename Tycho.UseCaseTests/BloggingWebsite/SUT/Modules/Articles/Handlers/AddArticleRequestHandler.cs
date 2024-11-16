using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Domain;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract.AddArticleRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Handlers;

internal class AddArticleRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddArticleRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response> Handle(AddArticleRequest requestData, CancellationToken cancellationToken)
    {
        var articles = _unitOfWork.Set<Article>();
        var newArticle = new Article(requestData.Author, requestData.Content);
        articles.Add(newArticle);
        await _unitOfWork.SaveChanges(cancellationToken);
        return new Response(newArticle.Id);
    }
}
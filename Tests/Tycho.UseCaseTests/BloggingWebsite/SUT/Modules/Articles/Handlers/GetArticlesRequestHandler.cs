using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract.GetArticlesRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Handlers;

internal class GetArticlesRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetArticlesRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response> Handle(GetArticlesRequest requestData, CancellationToken cancellationToken)
    {
        var articles = _unitOfWork.Set<Domain.Article>();
        var responseArticles = await articles
            .Where(article => requestData.ArticleIds.Contains(article.Id))
            .Select(article => new Article(
                article.Id, 
                article.Author, 
                article.Content))
            .ToArrayAsync(cancellationToken);
        return new Response(responseArticles);
    }
}
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Persistence;
using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract.GetArticlesRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Handlers;

internal class GetArticlesRequestHandler(ArticlesDbContext dbContext) : IRequestHandler<GetArticlesRequest, Response>
{
    public async Task<Response> HandleAsync(GetArticlesRequest requestData, CancellationToken cancellationToken)
    {
        Article[] responseArticles = await dbContext.Articles
            .Where(article => requestData.ArticleIds.Contains(article.Id))
            .Select(article => new Article(
                article.Id,
                article.Author,
                article.Content))
            .ToArrayAsync(cancellationToken);
        return new Response(responseArticles);
    }
}

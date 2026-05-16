using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Persistence;
using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract.AddArticleRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Handlers;

internal class AddArticleRequestHandler(ArticlesDbContext dbContext) : IRequestHandler<AddArticleRequest, Response>
{
    public async Task<Response> HandleAsync(AddArticleRequest requestData, CancellationToken cancellationToken)
    {
        var newArticle = new Article(requestData.Author, requestData.Content);
        dbContext.Articles.Add(newArticle);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new Response(newArticle.Id);
    }
}

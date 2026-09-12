using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract.GetArticlesRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract;

public record GetArticlesRequest(IReadOnlyList<int> ArticleIds) : IRequest<Response>
{
    public record Response(IReadOnlyList<Article> Articles);

    public record Article(int Id, string Author, string Content);
}

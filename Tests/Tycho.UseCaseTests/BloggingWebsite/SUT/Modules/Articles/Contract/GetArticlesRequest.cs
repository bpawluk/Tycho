using Tycho.Requests;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract.GetArticlesRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract;

public record GetArticlesRequest(IReadOnlyList<int> ArticleIds) : IRequest<Response>
{
    public record Response(IReadOnlyList<Article> Articles);

    public record Article(int Id, string Author, string Content);
}
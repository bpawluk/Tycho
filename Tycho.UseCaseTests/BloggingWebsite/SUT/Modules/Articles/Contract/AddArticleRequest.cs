using Tycho.Requests;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract.AddArticleRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract;

public record AddArticleRequest(string Author, string Content) : IRequest<Response>
{
    public record Response(int ArticleId);
}
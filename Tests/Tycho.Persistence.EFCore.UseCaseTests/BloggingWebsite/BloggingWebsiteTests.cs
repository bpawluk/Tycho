using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests._Utils;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite;

public sealed class BloggingWebsiteTests : IAsyncLifetime
{
    private readonly TestData _testData = new();
    private IBloggingWebsiteApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new BloggingWebsiteApp().RunAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task TychoUseCase_BloggingWebsiteApp_WorksCorrectly()
    {
        await SetupPostedEntries();

        var getMostDiscussedArticles = new GetFeedEntriesRequest(GetFeedEntriesRequest.ArticlesFeedData.MostDiscussed());
        GetFeedEntriesRequest.Response mostDiscussedArticles = await _sut.ExecuteAsync(getMostDiscussedArticles, TestContext.Current.CancellationToken);
        Assert.True(_testData.PostedEntries.Articles.MatchMostDiscussed(mostDiscussedArticles));

        int parentArticleId = _testData.PostedEntries.Articles.First().Id!.Value;
        var getMostDiscussedPosts = new GetFeedEntriesRequest(GetFeedEntriesRequest.PostsFeedData.MostDiscussed(parentArticleId));
        GetFeedEntriesRequest.Response mostDiscussedPosts = await _sut.ExecuteAsync(getMostDiscussedPosts, TestContext.Current.CancellationToken);
        Assert.True(_testData.PostedEntries.GetPosts(parentArticleId).MatchMostDiscussed(mostDiscussedPosts));

        int parentPostId = _testData.PostedEntries.Posts.Last().Id!.Value;
        var getMostDiscussedComments = new GetFeedEntriesRequest(GetFeedEntriesRequest.CommentsFeedData.MostDiscussed(parentPostId));
        GetFeedEntriesRequest.Response mostDiscussedComments = await _sut.ExecuteAsync(getMostDiscussedComments, TestContext.Current.CancellationToken);
        Assert.True(_testData.PostedEntries.GetComments(parentPostId).MatchMostDiscussed(mostDiscussedComments));

        await AddReactions();

        await AssertEventually.True(async () =>
        {
            var getMostLikedArticles = new GetFeedEntriesRequest(GetFeedEntriesRequest.ArticlesFeedData.MostLiked());
            GetFeedEntriesRequest.Response mostLikedArticles = await _sut.ExecuteAsync(getMostLikedArticles, TestContext.Current.CancellationToken);
            return _testData.PostedEntries.Articles.MatchMostLiked(mostLikedArticles);
        });

        await AssertEventually.True(async () =>
        {
            int parentArticleId = _testData.PostedEntries.Articles.First().Id!.Value;
            var getMostLikedPosts = new GetFeedEntriesRequest(GetFeedEntriesRequest.PostsFeedData.MostLiked(parentArticleId));
            GetFeedEntriesRequest.Response mostLikedPosts = await _sut.ExecuteAsync(getMostLikedPosts, TestContext.Current.CancellationToken);
            return _testData.PostedEntries.GetPosts(parentArticleId).MatchMostLiked(mostLikedPosts);
        });

        await AssertEventually.True(async () =>
        {
            int parentPostId = _testData.PostedEntries.Posts.Last().Id!.Value;
            var getMostLikedComments = new GetFeedEntriesRequest(GetFeedEntriesRequest.CommentsFeedData.MostLiked(parentPostId));
            GetFeedEntriesRequest.Response mostLikedComments = await _sut.ExecuteAsync(getMostLikedComments, TestContext.Current.CancellationToken);
            return _testData.PostedEntries.GetComments(parentPostId).MatchMostLiked(mostLikedComments);
        });
    }

    private async Task SetupPostedEntries()
    {
        foreach (TestData.Entry article in _testData.PostedEntries)
        {
            var articleEntry = new AddEntryRequest.ArticleEntryData(article.Author, article.Content);
            AddEntryRequest.Response addArticleResponse = await _sut.ExecuteAsync(new AddEntryRequest(articleEntry), TestContext.Current.CancellationToken);
            article.Id = addArticleResponse.AddedEntryId;

            foreach (TestData.Entry post in article.SubEntries)
            {
                var postEntry = new AddEntryRequest.PostEntryData(article.Id.Value, post.Author, post.Content);
                AddEntryRequest.Response addPostResponse = await _sut.ExecuteAsync(new AddEntryRequest(postEntry), TestContext.Current.CancellationToken);
                post.Id = addPostResponse.AddedEntryId;

                foreach (TestData.Entry comment in post.SubEntries)
                {
                    var commentEntry = new AddEntryRequest.CommentEntryData(post.Id.Value, comment.Author, comment.Content);
                    AddEntryRequest.Response addCommentResponse = await _sut.ExecuteAsync(new AddEntryRequest(commentEntry), TestContext.Current.CancellationToken);
                    comment.Id = addCommentResponse.AddedEntryId;
                }
            }
        }
    }

    private async Task AddReactions()
    {
        foreach (TestData.Reactions reactions in _testData.GetReactions())
        {
            for (int i = 0; i < reactions.Count; i++)
            {
                var addReactionRequest = new AddReactionRequest(reactions.TargetId);
                await _sut.ExecuteAsync(addReactionRequest, TestContext.Current.CancellationToken);
            }
            _testData.PostedEntries.Find(reactions.TargetId)!.Score += reactions.Count;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}

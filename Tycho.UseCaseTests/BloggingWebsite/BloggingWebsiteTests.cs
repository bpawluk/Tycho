using Tycho.Structure;
using Tycho.UseCaseTests._Utils;
using Tycho.UseCaseTests.BloggingWebsite.SUT;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Incoming;

namespace Tycho.UseCaseTests.BloggingWebsite;

public class BloggingWebsiteTests : IAsyncLifetime
{
    private readonly TestData _testData = new();
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new BloggingWebsiteApp().Run();
    }

    [Fact(Timeout = 2500)]
    public async Task TychoUseCase_BloggingWebsiteApp_WorksCorrectly() 
    {
        await SetupPostedEntries();

        var getMostDiscussedArticles = new GetFeedEntriesRequest(GetFeedEntriesRequest.ArticlesFeedData.MostDiscussed());
        var mostDiscussedArticles = await _sut.Execute<GetFeedEntriesRequest, GetFeedEntriesRequest.Response>(getMostDiscussedArticles);
        Assert.True(_testData.PostedEntries.Articles.MatchMostDiscussed(mostDiscussedArticles));

        var parentArticleId = _testData.PostedEntries.Articles.First().Id!.Value;
        var getMostDiscussedPosts = new GetFeedEntriesRequest(GetFeedEntriesRequest.PostsFeedData.MostDiscussed(parentArticleId));
        var mostDiscussedPosts = await _sut.Execute<GetFeedEntriesRequest, GetFeedEntriesRequest.Response>(getMostDiscussedPosts);
        Assert.True(_testData.PostedEntries.GetPosts(parentArticleId).MatchMostDiscussed(mostDiscussedPosts));

        var parentPostId = _testData.PostedEntries.Posts.Last().Id!.Value;
        var getMostDiscussedComments = new GetFeedEntriesRequest(GetFeedEntriesRequest.CommentsFeedData.MostDiscussed(parentPostId));
        var mostDiscussedComments = await _sut.Execute<GetFeedEntriesRequest, GetFeedEntriesRequest.Response>(getMostDiscussedComments);
        Assert.True(_testData.PostedEntries.GetComments(parentPostId).MatchMostDiscussed(mostDiscussedComments));

        await AddReactions();

        await AssertEventually.True(async () =>
        {
            var getMostLikedArticles = new GetFeedEntriesRequest(GetFeedEntriesRequest.ArticlesFeedData.MostLiked());
            var mostLikedArticles = await _sut.Execute<GetFeedEntriesRequest, GetFeedEntriesRequest.Response>(getMostLikedArticles);
            return _testData.PostedEntries.Articles.MatchMostLiked(mostLikedArticles);
        });

        await AssertEventually.True(async () =>
        {
            var parentArticleId = _testData.PostedEntries.Articles.First().Id!.Value;
            var getMostLikedPosts = new GetFeedEntriesRequest(GetFeedEntriesRequest.PostsFeedData.MostLiked(parentArticleId));
            var mostLikedPosts = await _sut.Execute<GetFeedEntriesRequest, GetFeedEntriesRequest.Response>(getMostLikedPosts);
            return _testData.PostedEntries.GetPosts(parentArticleId).MatchMostLiked(mostLikedPosts);
        });

        await AssertEventually.True(async () =>
        {
            var parentPostId = _testData.PostedEntries.Posts.Last().Id!.Value;
            var getMostLikedComments = new GetFeedEntriesRequest(GetFeedEntriesRequest.CommentsFeedData.MostLiked(parentPostId));
            var mostLikedComments = await _sut.Execute<GetFeedEntriesRequest, GetFeedEntriesRequest.Response>(getMostLikedComments);
            return _testData.PostedEntries.GetComments(parentPostId).MatchMostLiked(mostLikedComments);
        });
    }

    private async Task SetupPostedEntries()
    {
        foreach (var article in _testData.PostedEntries)
        {
            var articleEntry = new AddEntryRequest.ArticleEntryData(article.Author, article.Content);
            var addArticleResponse = await _sut.Execute<AddEntryRequest, AddEntryRequest.Response>(new(articleEntry));
            article.Id = addArticleResponse.AddedEntryId;

            foreach (var post in article.SubEntries)
            {
                var postEntry = new AddEntryRequest.PostEntryData(article.Id.Value, post.Author, post.Content);
                var addPostResponse = await _sut.Execute<AddEntryRequest, AddEntryRequest.Response>(new(postEntry));
                post.Id = addPostResponse.AddedEntryId;

                foreach (var comment in post.SubEntries)
                {
                    var commentEntry = new AddEntryRequest.CommentEntryData(post.Id.Value, comment.Author, comment.Content);
                    var addCommentResponse = await _sut.Execute<AddEntryRequest, AddEntryRequest.Response>(new(commentEntry));
                    comment.Id = addCommentResponse.AddedEntryId;
                }
            }
        }
    }

    private async Task AddReactions()
    {
        foreach (var reactions in _testData.GetReactions())
        {
            for (int i = 0; i < reactions.Count; i++)
            {
                var addReactionRequest = new AddReactionRequest(reactions.TargetId);
                await _sut.Execute(addReactionRequest);
            }
            _testData.PostedEntries.Find(reactions.TargetId)!.Score += reactions.Count;
        }
    }

    public async Task DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}
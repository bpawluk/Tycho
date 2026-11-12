using Tycho.Modules.Instance;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

internal class ContentRepository(
    IArticlesModule articlesModule,
    IPostsModule postsModule,
    ICommentsModule commentsModule)
{
    private readonly IArticlesModule _articlesModule = articlesModule;
    private readonly IPostsModule _postsModule = postsModule;
    private readonly ICommentsModule _commentsModule = commentsModule;

    public async Task<int> AddEntryContent(EntryType type, Content content)
    {
        if (type is EntryType.Article)
        {
            var addArticleRequest = new AddArticleRequest(content.Author, content.Value);
            AddArticleRequest.Response result = await _articlesModule.ExecuteAsync(addArticleRequest);
            return result.ArticleId;
        }
        else if (type is EntryType.Post)
        {
            var addPostRequest = new AddPostRequest(content.Author, content.Value);
            AddPostRequest.Response result = await _postsModule.ExecuteAsync(addPostRequest);
            return result.PostId;
        }
        else if (type is EntryType.Comment)
        {
            var addCommentRequest = new AddCommentRequest(content.Author, content.Value);
            AddCommentRequest.Response result = await _commentsModule.ExecuteAsync(addCommentRequest);
            return result.CommentId;
        }
        else
        {
            throw new ArgumentException($"Invalid entry type {type}", nameof(type));
        }
    }

    public async Task<IReadOnlyList<Content>> GetEntriesContents(EntryType type, IReadOnlyList<int> entryIds)
    {
        if (type is EntryType.Article)
        {
            var getArticlesRequest = new GetArticlesRequest(entryIds);
            GetArticlesRequest.Response result = await _articlesModule.ExecuteAsync(getArticlesRequest);
            return result.Articles.Select(article => new Content(article.Id, article.Author, article.Content)).ToArray();
        }
        else if (type is EntryType.Post)
        {
            var getPostsRequest = new GetPostsRequest(entryIds);
            GetPostsRequest.Response result = await _postsModule.ExecuteAsync(getPostsRequest);
            return result.Posts.Select(post => new Content(post.Id, post.Author, post.Content)).ToArray();
        }
        else if (type is EntryType.Comment)
        {
            var getCommentsRequest = new GetCommentsRequest(entryIds);
            GetCommentsRequest.Response result = await _commentsModule.ExecuteAsync(getCommentsRequest);
            return result.Comments.Select(comment => new Content(comment.Id, comment.Author, comment.Content)).ToArray();
        }
        else
        {
            throw new ArgumentException($"Invalid entry type {type}", nameof(type));
        }
    }
}

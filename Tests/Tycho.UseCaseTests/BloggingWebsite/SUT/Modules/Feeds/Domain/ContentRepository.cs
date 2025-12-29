using Tycho.Structure;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

internal class ContentRepository(
    IModule<ArticlesModule> articlesModule,
    IModule<PostsModule> postsModule,
    IModule<CommentsModule> commentsModule)
{
    private readonly IModule<ArticlesModule> _articlesModule = articlesModule;
    private readonly IModule<PostsModule> _postsModule = postsModule;
    private readonly IModule<CommentsModule> _commentsModule = commentsModule;

    public async Task<int> AddEntryContent(EntryType type, Content content)
    {
        if (type is EntryType.Article)
        {
            var addArticleRequest = new AddArticleRequest(content.Author, content.Value);
            var result = await _articlesModule.Execute<AddArticleRequest, AddArticleRequest.Response>(addArticleRequest);
            return result.ArticleId;
        }
        else if (type is EntryType.Post)
        {
            var addPostRequest = new AddPostRequest(content.Author, content.Value);
            var result = await _postsModule.Execute<AddPostRequest, AddPostRequest.Response>(addPostRequest);
            return result.PostId;
        }
        else if (type is EntryType.Comment)
        {
            var addCommentRequest = new AddCommentRequest(content.Author, content.Value);
            var result = await _commentsModule.Execute<AddCommentRequest, AddCommentRequest.Response>(addCommentRequest);
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
            var result = await _articlesModule.Execute<GetArticlesRequest, GetArticlesRequest.Response>(getArticlesRequest);
            return result.Articles.Select(article => new Content(article.Id, article.Author, article.Content)).ToArray();
        }
        else if (type is EntryType.Post)
        {
            var getPostsRequest = new GetPostsRequest(entryIds);
            var result = await _postsModule.Execute<GetPostsRequest, GetPostsRequest.Response>(getPostsRequest);
            return result.Posts.Select(post => new Content(post.Id, post.Author, post.Content)).ToArray();
        }
        else if (type is EntryType.Comment)
        {
            var getCommentsRequest = new GetCommentsRequest(entryIds);
            var result = await _commentsModule.Execute<GetCommentsRequest, GetCommentsRequest.Response>(getCommentsRequest);
            return result.Comments.Select(comment => new Content(comment.Id, comment.Author, comment.Content)).ToArray();
        }
        else
        {
            throw new ArgumentException($"Invalid entry type {type}", nameof(type));
        }
    }
}
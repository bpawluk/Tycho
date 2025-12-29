using Tycho.Requests;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract.GetFeedEntriesRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;

public record GetFeedEntriesRequest(FeedData Feed) : IRequest<Response>
{
    public record Response(IReadOnlyList<EntryData> Entries);

    public record EntryData(int Id, string Author, string Content, DateTime Created, uint Score, uint DiscussionWeight);

    public abstract record FeedData(FeedOrder Order);

    public record ArticlesFeedData : FeedData
    {
        private ArticlesFeedData(FeedOrder order) : base(order) { }

        public static ArticlesFeedData Latest() => new(FeedOrder.Latest);

        public static ArticlesFeedData MostLiked() => new(FeedOrder.MostLiked);

        public static ArticlesFeedData MostDiscussed() => new(FeedOrder.MostDiscussed);
    }

    public record PostsFeedData : FeedData
    {
        public int FeedId { get; init; }

        private PostsFeedData(int feedId, FeedOrder order) : base(order) 
        { 
            FeedId = feedId;
        }

        public static PostsFeedData Latest(int feedId) => new(feedId, FeedOrder.Latest);

        public static PostsFeedData MostLiked(int feedId) => new(feedId, FeedOrder.MostLiked);

        public static PostsFeedData MostDiscussed(int feedId) => new(feedId, FeedOrder.MostDiscussed);
    }

    public record CommentsFeedData : FeedData
    {
        public int FeedId { get; init; }

        private CommentsFeedData(int feedId, FeedOrder order) : base(order) 
        { 
            FeedId = feedId;
        }

        public static CommentsFeedData Latest(int feedId) => new(feedId, FeedOrder.Latest);

        public static CommentsFeedData MostLiked(int feedId) => new(feedId, FeedOrder.MostLiked);

        public static CommentsFeedData MostDiscussed(int feedId) => new(feedId, FeedOrder.MostDiscussed);
    }

    public enum FeedOrder
    {
        Latest,
        MostLiked,
        MostDiscussed
    }
}
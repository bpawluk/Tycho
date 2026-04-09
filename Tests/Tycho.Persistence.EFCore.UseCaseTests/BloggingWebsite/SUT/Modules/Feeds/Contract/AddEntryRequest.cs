using Tycho.Requests;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract.AddEntryRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;

public record AddEntryRequest(EntryData Entry) : IRequest<Response>
{
    public record Response(int AddedEntryId);

    public abstract record EntryData(string Author, string Content);

    public record ArticleEntryData : EntryData
    {
        public ArticleEntryData(string author, string content) : base(author, content) { }
    }

    public record PostEntryData : EntryData
    {
        public int FeedId { get; init; }

        public PostEntryData(int feedId, string author, string content) : base(author, content) 
        {
            FeedId = feedId;
        }
    }

    public record CommentEntryData : EntryData
    {
        public int FeedId { get; init; }

        public CommentEntryData(int feedId, string author, string content) : base(author, content)
        {
            FeedId = feedId;
        }
    }
}
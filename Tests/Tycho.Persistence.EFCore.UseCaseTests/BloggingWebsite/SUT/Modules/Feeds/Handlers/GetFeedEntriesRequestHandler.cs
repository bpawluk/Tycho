using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Persistence;
using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract.GetFeedEntriesRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Handlers;

internal class GetFeedEntriesRequestHandler(FeedsDbContext dbContext, ContentRepository contentRepository) : IRequestHandler<GetFeedEntriesRequest, Response>
{
    private readonly ContentRepository _contentRepository = contentRepository;

    public async Task<Response> HandleAsync(GetFeedEntriesRequest requestData, CancellationToken cancellationToken)
    {
        int? feedId = GetFeedId(requestData);
        var feedProvider = new FeedProvider(dbContext);
        Feed feed = await feedProvider.GetFeed(feedId, cancellationToken);

        IReadOnlyList<Entry> feedEntries = requestData.Feed.Order switch
        {
            FeedOrder.Latest => await feed.GetLatestEntries(cancellationToken),
            FeedOrder.MostLiked => await feed.GetMostLikedEntries(cancellationToken),
            FeedOrder.MostDiscussed => await feed.GetMostDiscussedEntries(cancellationToken),
            _ => throw new ArgumentException("Invalid feed order")
        };

        int[] contentIds = [.. feedEntries.Select(entry => entry.ContentId)];
        IReadOnlyList<Content> contents = await _contentRepository.GetEntriesContents(feed.EntriesType, contentIds);

        EntryData[] responseEntries = [.. feedEntries
            .Select((entry) =>
            {
                Content content = contents.First(content => content.Id == entry.ContentId);
                return new EntryData(
                    entry.Id,
                    content.Author,
                    content.Value,
                    entry.Created,
                    entry.Score,
                    entry.DiscussionWeight!.Value);
            })];

        return new Response(responseEntries);
    }

    private static int? GetFeedId(GetFeedEntriesRequest requestData)
    {
        return requestData.Feed switch
        {
            ArticlesFeedData => null,
            PostsFeedData postData => postData.FeedId,
            CommentsFeedData commentData => commentData.FeedId,
            _ => throw new ArgumentException("Invalid entry type")
        };
    }
}

using Tycho.Requests;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Persistence;
using static Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract.GetFeedEntriesRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Handlers;

internal class GetFeedEntriesRequestHandler(FeedsDbContext dbContext, ContentRepository contentRepository) : IRequestHandler<GetFeedEntriesRequest, Response>
{
    private readonly ContentRepository _contentRepository = contentRepository;

    public async Task<Response> HandleAsync(GetFeedEntriesRequest requestData, CancellationToken cancellationToken)
    {
        var feedId = GetFeedId(requestData);
        var feedProvider = new FeedProvider(dbContext);
        var feed = await feedProvider.GetFeed(feedId, cancellationToken);

        var feedEntries = requestData.Feed.Order switch
        {
            FeedOrder.Latest => await feed.GetLatestEntries(cancellationToken),
            FeedOrder.MostLiked => await feed.GetMostLikedEntries(cancellationToken),
            FeedOrder.MostDiscussed => await feed.GetMostDiscussedEntries(cancellationToken),
            _ => throw new ArgumentException("Invalid feed order")
        };

        var contentIds = feedEntries.Select(entry => entry.ContentId).ToArray();
        var contents = await _contentRepository.GetEntriesContents(feed.EntriesType, contentIds);

        var responseEntries = feedEntries
            .Select((entry) =>
            {
                var content = contents.First(content => content.Id == entry.ContentId);
                return new EntryData(
                    entry.Id,
                    content.Author,
                    content.Value,
                    entry.Created,
                    entry.Score,
                    entry.DiscussionWeight!.Value);
            })
            .ToArray();

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

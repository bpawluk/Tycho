using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Persistence;
using Tycho.Requests;
using static Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract.AddEntryRequest;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Handlers;

internal class AddEntryRequestHandler(FeedsDbContext dbContext, FeedProvider feedProvider, ContentRepository contentRepository) : IRequestHandler<AddEntryRequest, Response>
{
    public async Task<Response> HandleAsync(AddEntryRequest requestData, CancellationToken cancellationToken)
    {
        EntryType entryType = GetEntryType(requestData);
        var entryContent = new Content(requestData.Entry.Author, requestData.Entry.Content);
        int contentId = await contentRepository.AddEntryContent(entryType, entryContent);

        int? feedId = GetFeedId(requestData);
        Feed feed = await feedProvider.GetFeed(feedId, cancellationToken);

        Entry newEntry = feed.AddEntry(entryType, contentId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new Response(newEntry.Id);
    }

    private static EntryType GetEntryType(AddEntryRequest requestData)
    {
        return requestData.Entry switch
        {
            ArticleEntryData => EntryType.Article,
            PostEntryData => EntryType.Post,
            CommentEntryData => EntryType.Comment,
            _ => throw new ArgumentException("Invalid entry type")
        };
    }

    private static int? GetFeedId(AddEntryRequest requestData)
    {
        return requestData.Entry switch
        {
            ArticleEntryData => null,
            PostEntryData postData => postData.FeedId,
            CommentEntryData commentData => commentData.FeedId,
            _ => throw new ArgumentException("Invalid entry type")
        };
    }
}

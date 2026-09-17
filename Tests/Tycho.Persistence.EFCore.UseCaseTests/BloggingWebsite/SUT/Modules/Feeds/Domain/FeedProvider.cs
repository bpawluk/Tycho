using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

internal class FeedProvider(FeedsDbContext feedsDbContext)
{
    public async Task<Feed> GetFeed(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return new Feed(string.Empty, EntryType.Article, feedsDbContext);
        }

        Entry? feedOwner = await feedsDbContext.Entries.FindAsync([id], cancellationToken);

        if (feedOwner is not null)
        {
            return new Feed(feedOwner.SubfeedPath, GetFeedEntriesType(feedOwner.Type), feedsDbContext);
        }

        throw new InvalidOperationException("Requested Feed does not exist");
    }

    public static EntryType GetFeedEntriesType(EntryType feedOwnerType)
    {
        return feedOwnerType switch
        {
            EntryType.Article => EntryType.Post,
            EntryType.Post => EntryType.Comment,
            _ => throw new ArgumentException($"{feedOwnerType} Entry does not define a Feed")
        };
    }
}

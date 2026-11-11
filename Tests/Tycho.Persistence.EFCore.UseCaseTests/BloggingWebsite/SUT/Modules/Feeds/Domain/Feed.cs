using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

internal class Feed(string path, EntryType entriesType, FeedsDbContext feedsDbContext)
{
    private readonly string _path = path;

    public EntryType EntriesType { get; private set; } = entriesType;

    public Entry AddEntry(EntryType entryType, int contentId)
    {
        if (entryType != EntriesType)
        {
            throw new InvalidOperationException("Entry type does not match the Feed");
        }

        var newEntry = new Entry(contentId, _path, entryType);
        feedsDbContext.Entries.Add(newEntry);

        return newEntry;
    }

    public Task<IReadOnlyList<FeedEntry>> GetLatestEntries(CancellationToken cancellationToken)
        => GetEntries(FeedOrder.Latest, cancellationToken);

    public Task<IReadOnlyList<FeedEntry>> GetMostLikedEntries(CancellationToken cancellationToken)
        => GetEntries(FeedOrder.MostLiked, cancellationToken);

    public Task<IReadOnlyList<FeedEntry>> GetMostDiscussedEntries(CancellationToken cancellationToken)
        => GetEntries(FeedOrder.MostDiscussed, cancellationToken);

    private async Task<IReadOnlyList<FeedEntry>> GetEntries(FeedOrder order, CancellationToken cancellationToken)
    {
        var entries = feedsDbContext.Entries
            .AsNoTracking()
            .Where(entry => entry.Type == EntriesType && entry.FeedPath == _path)
            .Select(entry => new
            {
                Entry = entry,
                DiscussionWeight = (uint)feedsDbContext.Entries.Count(subentry =>
                    EF.Functions.Like(subentry.FeedPath, entry.FeedPath + "/" + entry.Id.ToString() + "%"))
            });

        var sorted = order switch
        {
            FeedOrder.Latest => entries.OrderByDescending(item => item.Entry.Created),
            FeedOrder.MostLiked => entries.OrderByDescending(item => item.Entry.Score),
            FeedOrder.MostDiscussed => entries.OrderByDescending(item => item.DiscussionWeight),
            _ => throw new ArgumentOutOfRangeException(nameof(order))
        };

        return await sorted
            .Select(item => new FeedEntry(item.Entry.Id, item.Entry.ContentId,
                item.Entry.Created, item.Entry.Score, item.DiscussionWeight))
            .ToArrayAsync(cancellationToken);
    }

    private enum FeedOrder { Latest, MostLiked, MostDiscussed }
}

internal record FeedEntry(int Id, int ContentId, DateTime Created, uint Score, uint DiscussionWeight);

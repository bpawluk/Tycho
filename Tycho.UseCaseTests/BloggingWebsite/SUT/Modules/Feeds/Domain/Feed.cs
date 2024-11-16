using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

internal class Feed(string path, EntryType entriesType, IUnitOfWork unitOfWork)
{
    private readonly string _path = path;
    private readonly DbSet<Entry> _entries = unitOfWork.Set<Entry>();

    public EntryType EntriesType { get; private set; } = entriesType;

    public Entry AddEntry(EntryType entryType, int contentId)
    {
        if (entryType != EntriesType)
        {
            throw new InvalidOperationException("Entry type does not match the Feed");
        }

        var newEntry = new Entry(contentId, _path, entryType);
        _entries.Add(newEntry);

        return newEntry;
    }

    public async Task<IReadOnlyList<Entry>> GetLatestEntries(CancellationToken cancellationToken)
    {
        return await _entries
            .FromSqlRaw(@"
                SELECT 
                    entry.Id,
                    entry.ContentId,
                    entry.FeedPath,
                    entry.Type,
                    entry.Created,
                    entry.Score,
                    COUNT(subentry.Id) AS DiscussionWeight
                FROM Entries AS entry
                LEFT JOIN Entries 
                    AS subentry
                    ON subentry.FeedPath LIKE entry.FeedPath || '/' || entry.Id || '%'
                WHERE 
                    entry.Type = {0} AND 
                    entry.FeedPath = {1}
                GROUP BY entry.Id
                ORDER BY entry.Created DESC;", EntriesType, _path)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Entry>> GetMostLikedEntries(CancellationToken cancellationToken)
    {
        return await _entries
            .FromSqlRaw(@"
                SELECT 
                    entry.Id,
                    entry.ContentId,
                    entry.FeedPath,
                    entry.Type,
                    entry.Created,
                    entry.Score,
                    COUNT(subentry.Id) AS DiscussionWeight
                FROM Entries AS entry
                LEFT JOIN Entries 
                    AS subentry
                    ON subentry.FeedPath LIKE entry.FeedPath || '/' || entry.Id || '%'
                WHERE 
                    entry.Type = {0} AND 
                    entry.FeedPath = {1}
                GROUP BY entry.Id
                ORDER BY entry.Score DESC;", EntriesType, _path)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Entry>> GetMostDiscussedEntries(CancellationToken cancellationToken)
    {
        return await _entries
            .FromSqlRaw(@"
                SELECT 
                    entry.Id,
                    entry.ContentId,
                    entry.FeedPath,
                    entry.Type,
                    entry.Created,
                    entry.Score,
                    COUNT(subentry.Id) AS DiscussionWeight
                FROM Entries AS entry
                LEFT JOIN Entries 
                    AS subentry
                    ON subentry.FeedPath LIKE entry.FeedPath || '/' || entry.Id || '%'
                WHERE 
                    entry.Type = {0} AND 
                    entry.FeedPath = {1}
                GROUP BY entry.Id
                ORDER BY DiscussionWeight DESC;", EntriesType, _path)
            .ToArrayAsync(cancellationToken);
    }
}
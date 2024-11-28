using Tycho.Persistence.EFCore;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

internal class FeedProvider(IUnitOfWork unitOfWork)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Feed> GetFeed(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return new Feed(string.Empty, EntryType.Article, _unitOfWork);
        }

        var entries = _unitOfWork.Set<Entry>();
        var feedOwner = await entries.FindAsync([id], cancellationToken);

        if (feedOwner is not null)
        {
            return new Feed(feedOwner.SubfeedPath, GetFeedEntriesType(feedOwner.Type), _unitOfWork);
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
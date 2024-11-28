namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

internal class Entry(int contentId, string feedPath, EntryType type)
{
    public int Id { get; private set; }

    public int ContentId { get; private set; } = contentId;

    public string FeedPath { get; private set; } = feedPath;

    public EntryType Type { get; private set; } = type;

    public DateTime Created { get; private set; } = DateTime.UtcNow;

    public uint Score { get; private set; } = 0;

    public uint? DiscussionWeight { get; private set; }

    public string SubfeedPath => $"{FeedPath}/{Id}";

    public void UpdateScore(uint newScore)
    {
        if (newScore > Score)
        {
            Score = newScore;
        }
    }
}
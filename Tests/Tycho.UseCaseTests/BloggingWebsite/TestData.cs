using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;

namespace Tycho.UseCaseTests.BloggingWebsite;

internal class TestData
{
    public Entries PostedEntries { get; } =
    [
        new("Jenny", "An Article with multiple Posts",
        [
            new("Bart", "A Post without Comments", []),
            new("Michael", "Other Post with a single Comments",
            [
                new("Adam", "A Comment", [])
            ]),
            new("Eric", "Another Post with two Comments",
            [
                new("Jenny", "A Comment", []),
                new("Michael", "A Comment", [])
            ]),
        ]),
        new("Adam", "An Article without Posts", []),
        new("Mike", "An Article with a single Post with a single Comment", 
        [
            new("Adam", "A Post with three Comments", 
            [
                new("Eric", "A Comment", []),
                new("Mike", "A Comment", []),
                new("Adam", "A Comment", []),
            ])
        ]),
    ];

    public List<Reactions> GetReactions() =>
    [
        new(PostedEntries[0].Id!.Value, 1),
        new(PostedEntries[1].Id!.Value, 3),
        new(PostedEntries[2].Id!.Value, 2),
        new(PostedEntries[0].SubEntries[0].Id!.Value, 2),
        new(PostedEntries[0].SubEntries[1].Id!.Value, 1),
        new(PostedEntries[2].SubEntries[0].SubEntries[1].Id!.Value, 1),
        new(PostedEntries[2].SubEntries[0].SubEntries[2].Id!.Value, 2),
    ];

    public class Entries : List<Entry>
    {
        public Entries Articles => this;

        public Entries Posts => [.. this.SelectMany(entry => entry.SubEntries)];

        public Entries Comments => [.. Posts.SelectMany(entry => entry.SubEntries)];

        public Entry? Find(int id)
        {
            foreach (var entry in this)
            {
                if (entry.Id == id)
                {
                    return entry;
                }

                var found = entry.SubEntries.Find(id);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public Entries GetPosts(int articleId)
        {
            return Articles.Single(article => article.Id == articleId).SubEntries;
        }

        public Entries GetComments(int postId)
        {
            return Posts.Single(post => post.Id == postId).SubEntries;
        }

        public bool MatchMostLiked(GetFeedEntriesRequest.Response response)
        {
            return Match([.. this.OrderByDescending(entry => entry.Score)], response);

        }

        public bool MatchMostDiscussed(GetFeedEntriesRequest.Response response)
        {
            return Match([.. this.OrderByDescending(entry => entry.DiscussionWeight)], response);

        }

        private static bool Match(IReadOnlyList<Entry> entries, GetFeedEntriesRequest.Response response)
        {
            if (entries.Count != response.Entries.Count)
            {
                return false;
            }

            foreach (var (entry, responseEntry) in entries.Zip(response.Entries))
            {
                if (!entry.Matches(responseEntry))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public record Entry(string Author, string Content, Entries SubEntries)
    {
        public int? Id { get; set; }

        public uint Score { get; set; } = 0;

        public int DiscussionWeight => SubEntries.SelectMany(entry => entry.SubEntries.Prepend(entry)).Count();

        public bool Matches(GetFeedEntriesRequest.EntryData fetchedEntry)
        {
            return Id == fetchedEntry.Id &&
                   Author == fetchedEntry.Author &&
                   Content == fetchedEntry.Content &&
                   Score == fetchedEntry.Score &&
                   DiscussionWeight == fetchedEntry.DiscussionWeight;
        }
    }

    public record Reactions(int TargetId, uint Count);
}
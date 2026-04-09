using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

namespace Tycho.UseCaseTests.ContentModeration;

internal class TestData
{
    private Posts? _initialPosts = null!;

    public Users InitialUsers { get; } =
    [
        new("Alice"),
        new("Bob"),
        new("Charlie"),
    ];

    public Posts GetInitialPosts()
    {
        if (_initialPosts == null)
        {
            _initialPosts =
            [
                new(InitialUsers[0].Id!.Value, "Alice's Post"),
                new(InitialUsers[0].Id!.Value, "Alice's Slightly Inappropriate Post"),
                new(InitialUsers[1].Id!.Value, "Bob's Post"),
                new(InitialUsers[1].Id!.Value, "Bob's Very Inappropriate Post"),
                new(InitialUsers[2].Id!.Value, "Charlie's Post"),
            ];
        }
        return _initialPosts;
    }

    public PostRemovals GetPostRemovals()
    {
        var posts = GetInitialPosts();
        return
        [
            new(posts[1], false),
            new(posts[3], true),
        ];
    }

    public Users GetUsersAfterPostRemovals()
    {
        var postRemovals = GetPostRemovals();
        return [.. InitialUsers.Where(user => !postRemovals.BannedUsersIds.Contains(user.Id!.Value))];
    }

    public Posts GetPostsAfterPostRemovals()
    {
        var postRemovals = GetPostRemovals();
        return [.. GetInitialPosts().Where(post => !postRemovals.RemovedPosts.Contains(post))];
    }

    public class Users : List<User>
    {
        public bool Match(GetUsersRequest.Response response)
        {
            return Count == response.Users.Count &&
                   this.All(item => response.Users.Any(item.Matches));
        }
    }

    public record User(string Name)
    {
        public int? Id { get; set; }

        public bool Matches(GetUsersRequest.User fetchedUser)
        {
            return Id == fetchedUser.Id && Name == fetchedUser.Name;
        }
    }

    public class Posts : List<Post>
    {
        public bool Match(GetPostsRequest.Response response)
        {
            return Count == response.Posts.Count &&
                   this.All(item => response.Posts.Any(item.Matches));
        }
    }

    public record Post(int AuthorId, string Content)
    {
        public int? Id { get; set; }

        public bool Matches(GetPostsRequest.Post fetchedPost)
        {
            return Id == fetchedPost.Id &&
                   AuthorId == fetchedPost.AuthorId &&
                   Content == fetchedPost.Content;
        }
    }

    public class PostRemovals : List<PostRemoval>
    {
        public Posts RemovedPosts => [.. this.Select(item => item.Post)];

        public IReadOnlyList<int> BannedUsersIds => this
            .Where(postRemoval => postRemoval.BanAuthor)
            .Select(postRemoval => postRemoval.Post.AuthorId)
            .ToArray();
    }

    public record PostRemoval(Post Post, bool BanAuthor);
}

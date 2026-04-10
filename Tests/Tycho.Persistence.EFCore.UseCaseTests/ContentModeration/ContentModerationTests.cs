using Tycho.UseCaseTests._Utils;
using Tycho.UseCaseTests.ContentModeration.SUT;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Incoming;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

namespace Tycho.UseCaseTests.ContentModeration;

public class ContentModerationTests : IAsyncLifetime
{
    private readonly TestData _testData = new();
    private IContentModerationApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new ContentModerationApp().RunAsync();
    }

    [Fact(Timeout = 2500)]
    public async Task TychoUseCase_ContentModerationApp_WorksCorrectly()
    {
        await SetupUsers();

        var users = await _sut.ExecuteAsync(new GetUsersRequest());
        Assert.True(_testData.InitialUsers.Match(users));

        await SetupPosts();

        var posts = await _sut.ExecuteAsync(new GetPostsRequest());
        Assert.True(_testData.GetInitialPosts().Match(posts));

        await RemoveInappropriatePosts();

        await AssertEventually.True(async () =>
        {
            var users = await _sut.ExecuteAsync(new GetUsersRequest());
            return _testData.GetUsersAfterPostRemovals().Match(users);
        });

        await AssertEventually.True(async () =>
        {
            var posts = await _sut.ExecuteAsync(new GetPostsRequest());
            return _testData.GetPostsAfterPostRemovals().Match(posts);
        });
    }

    private async Task SetupUsers()
    {
        foreach (var user in _testData.InitialUsers)
        {
            var addUserRequest = new AddUserRequest(user.Name);
            var response = await _sut.ExecuteAsync(addUserRequest);
            user.Id = response.UserId;
        }
    }

    private async Task SetupPosts()
    {
        foreach (var post in _testData.GetInitialPosts())
        {
            var addPostRequest = new AddPostRequest(post.AuthorId, post.Content);
            var response = await _sut.ExecuteAsync(addPostRequest);
            post.Id = response.PostId;
        }
    }

    private async Task RemoveInappropriatePosts()
    {
        foreach(var postRemoval in _testData.GetPostRemovals())
        {
            var removePostRequest = new RemovePostRequest(postRemoval.Post.Id!.Value, postRemoval.BanAuthor);
            await _sut.ExecuteAsync(removePostRequest);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}
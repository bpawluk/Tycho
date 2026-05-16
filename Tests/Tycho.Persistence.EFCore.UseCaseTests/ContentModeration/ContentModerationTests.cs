using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;
using Tycho.Persistence.EFCore.UseCaseTests._Utils;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration;

public sealed class ContentModerationTests : IAsyncLifetime
{
    private readonly TestData _testData = new();
    private IContentModerationApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new ContentModerationApp().RunAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task TychoUseCase_ContentModerationApp_WorksCorrectly()
    {
        await SetupUsers();

        GetUsersRequest.Response users = await _sut.ExecuteAsync(new GetUsersRequest(), TestContext.Current.CancellationToken);
        Assert.True(_testData.InitialUsers.Match(users));

        await SetupPosts();

        GetPostsRequest.Response posts = await _sut.ExecuteAsync(new GetPostsRequest(), TestContext.Current.CancellationToken);
        Assert.True(_testData.GetInitialPosts().Match(posts));

        await RemoveInappropriatePosts();

        await AssertEventually.True(async () =>
        {
            GetUsersRequest.Response users = await _sut.ExecuteAsync(new GetUsersRequest(), TestContext.Current.CancellationToken);
            return _testData.GetUsersAfterPostRemovals().Match(users);
        });

        await AssertEventually.True(async () =>
        {
            GetPostsRequest.Response posts = await _sut.ExecuteAsync(new GetPostsRequest(), TestContext.Current.CancellationToken);
            return _testData.GetPostsAfterPostRemovals().Match(posts);
        });
    }

    private async Task SetupUsers()
    {
        foreach (TestData.User user in _testData.InitialUsers)
        {
            var addUserRequest = new AddUserRequest(user.Name);
            AddUserRequest.Response response = await _sut.ExecuteAsync(addUserRequest, TestContext.Current.CancellationToken);
            user.Id = response.UserId;
        }
    }

    private async Task SetupPosts()
    {
        foreach (TestData.Post post in _testData.GetInitialPosts())
        {
            var addPostRequest = new AddPostRequest(post.AuthorId, post.Content);
            AddPostRequest.Response response = await _sut.ExecuteAsync(addPostRequest, TestContext.Current.CancellationToken);
            post.Id = response.PostId;
        }
    }

    private async Task RemoveInappropriatePosts()
    {
        foreach (TestData.PostRemoval postRemoval in _testData.GetPostRemovals())
        {
            var removePostRequest = new RemovePostRequest(postRemoval.Post.Id!.Value, postRemoval.BanAuthor);
            await _sut.ExecuteAsync(removePostRequest, TestContext.Current.CancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}

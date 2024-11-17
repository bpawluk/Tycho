using Tycho.Structure;
using Tycho.UseCaseTests._Utils;
using Tycho.UseCaseTests.ContentModeration.SUT;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Incoming;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

namespace Tycho.UseCaseTests.ContentModeration;

public class ContentModerationTests : IAsyncLifetime
{
    private readonly TestData _testData = new();
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new ContentModerationApp().Run();
    }

    [Fact(Timeout = 2500)]
    public async Task TychoUseCase_ContentModerationApp_WorksCorrectly()
    {
        await SetupUsers();

        var users = await _sut.Execute<GetUsersRequest, GetUsersRequest.Response>(new());
        Assert.True(_testData.InitialUsers.Match(users));

        await SetupPosts();

        var posts = await _sut.Execute<GetPostsRequest, GetPostsRequest.Response>(new());
        Assert.True(_testData.GetInitialPosts().Match(posts));

        await RemoveInappropriatePosts();

        await AssertEventually.True(async () =>
        {
            var users = await _sut.Execute<GetUsersRequest, GetUsersRequest.Response>(new());
            return _testData.GetUsersAfterPostRemovals().Match(users);
        });

        await AssertEventually.True(async () =>
        {
            var posts = await _sut.Execute<GetPostsRequest, GetPostsRequest.Response>(new());
            return _testData.GetPostsAfterPostRemovals().Match(posts);
        });
    }

    private async Task SetupUsers()
    {
        foreach (var user in _testData.InitialUsers)
        {
            var addUserRequest = new AddUserRequest(user.Name);
            var response = await _sut.Execute<AddUserRequest, AddUserRequest.Response>(addUserRequest);
            user.Id = response.UserId;
        }
    }

    private async Task SetupPosts()
    {
        foreach (var post in _testData.GetInitialPosts())
        {
            var addPostRequest = new AddPostRequest(post.AuthorId, post.Content);
            var response = await _sut.Execute<AddPostRequest, AddPostRequest.Response>(addPostRequest);
            post.Id = response.PostId;
        }
    }

    private async Task RemoveInappropriatePosts()
    {
        foreach(var postRemoval in _testData.GetPostRemovals())
        {
            var removePostRequest = new RemovePostRequest(postRemoval.Post.Id!.Value, postRemoval.BanAuthor);
            await _sut.Execute(removePostRequest);
        }
    }

    public Task DisposeAsync()
    {
        _sut?.Dispose();
        return Task.CompletedTask;
    }
}
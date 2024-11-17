using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Persistence;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts;

public class PostsModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<AddPostRequest, AddPostRequest.Response, AddPostRequestHandler>()
              .Handles<GetPostRequest, GetPostRequest.Response, GetPostRequestHandler>()
              .Handles<GetPostsRequest, GetPostsRequest.Response, GetPostsRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Handles<PostStatusChangedEvent, PostStatusChangedEventHandler>();
    }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<PostsDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using var context = app.GetRequiredService<PostsDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
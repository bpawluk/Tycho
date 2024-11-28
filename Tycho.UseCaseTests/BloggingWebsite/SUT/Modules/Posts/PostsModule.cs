using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Handlers;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Persistence;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts;

public class PostsModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<AddPostRequest, AddPostRequest.Response, AddPostRequestHandler>()
              .Handles<GetPostsRequest, GetPostsRequest.Response, GetPostsRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) { }

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
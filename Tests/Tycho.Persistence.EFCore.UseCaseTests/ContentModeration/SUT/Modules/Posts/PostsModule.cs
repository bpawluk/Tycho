using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts;

[TychoDefinition]
public partial class PostsModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<AddPostRequest, AddPostRequest.Response>()
              .HandlesWith<AddPostRequestHandler>();

        module.Expects<GetPostRequest, GetPostRequest.Response>()
              .HandlesWith<GetPostRequestHandler>();

        module.Expects<GetPostsRequest, GetPostsRequest.Response>()
              .HandlesWith<GetPostsRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<PostStatusChangedEvent>()
              .HandlesWith<PostStatusChangedEventHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<PostsDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using PostsDbContext context = app.GetRequiredService<PostsDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}

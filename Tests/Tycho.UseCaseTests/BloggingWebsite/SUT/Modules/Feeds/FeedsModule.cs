using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Handlers;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Persistence;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Posts;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds;

public class FeedsModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<AddEntryRequest, AddEntryRequest.Response, AddEntryRequestHandler>()
              .Handles<GetFeedEntriesRequest, GetFeedEntriesRequest.Response, GetFeedEntriesRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Handles<ScoreChangedEvent, ScoreChangedEventHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) 
    {
        module.Uses<ArticlesModule>()
              .Uses<PostsModule>()
              .Uses<CommentsModule>();
    }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<FeedsDbContext>()
              .AddTransient<ContentRepository>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using var context = app.GetRequiredService<FeedsDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
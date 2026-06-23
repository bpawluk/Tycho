using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Persistence;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds;

[TychoDefinition]
public partial class FeedsModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<AddEntryRequest, AddEntryRequest.Response>()
              .HandlesWith<AddEntryRequestHandler>();

        module.Expects<GetFeedEntriesRequest, GetFeedEntriesRequest.Response>()
              .HandlesWith<GetFeedEntriesRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<ScoreChangedEvent>()
              .HandlesWith<ScoreChangedEventHandler>();
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
              .AddTransient<ContentRepository>()
              .AddTransient<FeedProvider>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using FeedsDbContext context = app.GetRequiredService<FeedsDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}

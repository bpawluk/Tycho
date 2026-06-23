using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Incoming;
using Feeds = Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds;
using Reactions = Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT;

[TychoDefinition]
public partial class BloggingWebsiteApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Forwards<AddEntryRequest, AddEntryRequest.Response, FeedsModule>()
           .Forwards<GetFeedEntriesRequest, GetFeedEntriesRequest.Response, FeedsModule>();

        app.Forwards<AddReactionRequest, ReactionsModule>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Expects<Reactions.Contract.Outgoing.ScoreChangedEvent>()
           .MapsTo<Feeds.Contract.ScoreChangedEvent>(eventData => new(eventData.TargetId, eventData.NewScore))
           .ForwardsTo<FeedsModule>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<FeedsModule>()
           .Uses<ReactionsModule>();
    }

    protected override void RegisterServices(IServiceCollection app) { }
}

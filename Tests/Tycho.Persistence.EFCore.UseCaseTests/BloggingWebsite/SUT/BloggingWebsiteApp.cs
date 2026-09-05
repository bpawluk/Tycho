using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Incoming;
using FeedsIn = Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using ReactionsOut = Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Contract.Outgoing;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT;

[TychoDefinition]
public partial class BloggingWebsiteApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<AddEntryRequest, AddEntryRequest.Response>()
           .ForwardsTo<FeedsModule>();

        app.Expects<GetFeedEntriesRequest, GetFeedEntriesRequest.Response>()
           .ForwardsTo<FeedsModule>();

        app.Expects<AddReactionRequest>()
           .ForwardsTo<ReactionsModule>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Expects<ReactionsOut.ScoreChangedEvent>()
           .MapsTo<FeedsIn.ScoreChangedEvent>(payload => new(payload.TargetId, payload.NewScore))
           .ForwardsTo<FeedsModule>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<FeedsModule>()
           .Uses<ReactionsModule>();
    }

    protected override void RegisterServices(IServiceCollection app) { }
}

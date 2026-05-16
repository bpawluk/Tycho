using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments;

[TychoDefinition]
public partial class CommentsModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<AddCommentRequest, AddCommentRequest.Response, AddCommentRequestHandler>()
              .Handles<GetCommentsRequest, GetCommentsRequest.Response, GetCommentsRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<CommentsDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using var context = app.GetRequiredService<CommentsDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}

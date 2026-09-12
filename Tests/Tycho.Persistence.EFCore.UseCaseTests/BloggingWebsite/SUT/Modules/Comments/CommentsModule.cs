using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments;

[TychoDefinition]
public partial class CommentsModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<AddCommentRequest, AddCommentRequest.Response>()
              .HandlesWith<AddCommentRequestHandler>();

        module.Expects<GetCommentsRequest, GetCommentsRequest.Response>()
              .HandlesWith<GetCommentsRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<CommentsDbContext>();
    }

    protected override async Task Startup(IServiceProvider module, CancellationToken cancellationToken)
    {
        CommentsDbContext context = module.GetRequiredService<CommentsDbContext>();
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}

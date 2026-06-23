using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles;

[TychoDefinition]
public partial class ArticlesModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<AddArticleRequest, AddArticleRequest.Response>()
              .HandlesWith<AddArticleRequestHandler>();

        module.Expects<GetArticlesRequest, GetArticlesRequest.Response>()
              .HandlesWith<GetArticlesRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<ArticlesDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using ArticlesDbContext context = app.GetRequiredService<ArticlesDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}

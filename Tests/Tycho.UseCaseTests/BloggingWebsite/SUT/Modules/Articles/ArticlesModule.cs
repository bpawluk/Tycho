using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Handlers;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Persistence;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles;

public class ArticlesModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<AddArticleRequest, AddArticleRequest.Response, AddArticleRequestHandler>()
              .Handles<GetArticlesRequest, GetArticlesRequest.Response, GetArticlesRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<ArticlesDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using var context = app.GetRequiredService<ArticlesDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
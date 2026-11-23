using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog;

[TychoDefinition]
public partial class CatalogModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<AddProductToBasketRequest>();

        module.Expects<CreateProductRequest, CreateProductRequest.Response>()
              .HandlesWith<CreateProductRequestHandler>();

        module.Expects<GetProductsRequest, GetProductsRequest.Response>()
              .HandlesWith<GetProductsRequestHandler>();

        module.Expects<BuyProductRequest>()
              .HandlesWith<BuyProductRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<ProductAvailabilityChangedEvent>()
              .HandlesWith<ProductAvailabilityChangedEventHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<CatalogDbContext>();
    }

    protected override async Task Startup(IServiceProvider module, CancellationToken cancellationToken)
    {
        CatalogDbContext context = module.GetRequiredService<CatalogDbContext>();
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}

using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog;

internal class CatalogModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<CreateProductRequest, CreateProductRequest.Response, CreateProductRequestHandler>()
              .Handles<GetProductsRequest, GetProductsRequest.Response, GetProductsRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Handles<ProductAvailabilityChangedEvent, ProductAvailabilityChangedEventHandler>();
    }

    protected override void RegisterServices(IServiceCollection module)
    {

    }
}
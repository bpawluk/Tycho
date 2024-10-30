using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Handlers;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering;

internal class OrderingModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<GetOrdersRequest, GetOrdersRequest.Response, GetOrdersRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Handles<OrderPlacedEvent, OrderPlacedEventHandler>();
    }

    protected override void RegisterServices(IServiceCollection module)
    {

    }
}
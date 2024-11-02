using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Persistence;
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
        module.AddTychoPersistence<OrderingDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using var context = app.GetRequiredService<OrderingDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
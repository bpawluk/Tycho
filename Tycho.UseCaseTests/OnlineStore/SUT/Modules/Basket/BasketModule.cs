using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Persistence;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket;

internal class BasketModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<AddBasketItemRequest, AddBasketItemRequestHandler>()
              .Handles<ConfirmBasketItemRequest, ConfirmBasketItemRequestHandler>()
              .Handles<DeclineBasketItemRequest, DeclineBasketItemRequestHandler>()
              .Handles<GetBasketRequest, GetBasketRequest.Response, GetBasketRequestHandler>()
              .Handles<CheckoutRequest, CheckoutRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Routes<BasketItemAddedEvent>()
              .Exposes();

        module.Routes<BasketCheckedOutEvent>()
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<BasketDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using var context = app.GetRequiredService<BasketDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Events;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket;

internal class BasketModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<AddBasketItemRequest, AddBasketItemRequestHandler>()
              .Handles<ConfirmBasketItemRequest, ConfirmBasketItemRequestHandler>()
              .Handles<DeclineBasketItemRequest, DeclineBasketItemRequestHandler>()
              .Handles<CheckoutRequest, CheckoutRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Routes<BasketItemAddedEvent>()
              .Exposes();

        module.Routes<BasketCheckedOutEvent>()
              .Exposes();
    }

    protected override void RegisterServices(IServiceCollection module)
    {

    }
}
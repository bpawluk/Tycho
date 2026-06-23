using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket;

[TychoDefinition]
public partial class BasketModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<AddBasketItemRequest>()
              .HandlesWith<AddBasketItemRequestHandler>();

        module.Expects<ConfirmBasketItemRequest>()
              .HandlesWith<ConfirmBasketItemRequestHandler>();

        module.Expects<DeclineBasketItemRequest>()
              .HandlesWith<DeclineBasketItemRequestHandler>();

        module.Expects<GetBasketRequest, GetBasketRequest.Response>()
              .HandlesWith<GetBasketRequestHandler>();

        module.Expects<CheckoutRequest>()
              .HandlesWith<CheckoutRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<BasketItemAddedEvent>()
              .Exposes();

        module.Expects<BasketCheckedOutEvent>()
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<BasketDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using BasketDbContext context = app.GetRequiredService<BasketDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}

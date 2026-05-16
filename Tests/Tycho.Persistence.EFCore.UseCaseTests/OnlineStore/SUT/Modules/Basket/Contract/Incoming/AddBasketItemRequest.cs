using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;

public record AddBasketItemRequest(
    int CustomerId,
    int ProductId,
    uint Quantity,
    decimal Price)
    : IRequest;

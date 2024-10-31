using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;

public record AddBasketItemRequest(
    int CustomerId, 
    int ProductId, 
    uint Quantity, 
    decimal Price) 
    : IRequest;
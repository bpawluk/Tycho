using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;

public record DeclineBasketItemRequest(int CustomerId, int ProductId) : IRequest;
using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;

public record ConfirmBasketItemRequest(int CustomerId, int ProductId) : IRequest;
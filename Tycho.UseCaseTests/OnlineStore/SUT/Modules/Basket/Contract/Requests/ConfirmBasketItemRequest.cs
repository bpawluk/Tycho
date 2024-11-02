using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;

public record ConfirmBasketItemRequest(int CustomerId, int ProductId) : IRequest;
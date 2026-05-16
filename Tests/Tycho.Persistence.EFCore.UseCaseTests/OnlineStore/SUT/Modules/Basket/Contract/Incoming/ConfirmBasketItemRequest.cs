using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;

public record ConfirmBasketItemRequest(int CustomerId, int ProductId) : IRequest;

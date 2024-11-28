using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Outgoing;

public record AddProductToBasketRequest(
    int CustomerId,
    int ProductId,
    uint Quantity,
    decimal Price)
    : IRequest;
using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;

public record BuyProductRequest(
    int CustomerId,
    int ProductId,
    uint Quantity)
    : IRequest;
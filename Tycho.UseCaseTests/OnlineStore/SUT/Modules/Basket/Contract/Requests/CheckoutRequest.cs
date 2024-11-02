using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;

public record CheckoutRequest(int CustomerId) : IRequest;
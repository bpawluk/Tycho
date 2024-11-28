using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;

public record CheckoutRequest(int CustomerId) : IRequest;
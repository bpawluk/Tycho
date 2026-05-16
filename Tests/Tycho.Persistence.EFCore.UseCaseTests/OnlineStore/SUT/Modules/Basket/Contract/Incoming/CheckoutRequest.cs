using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;

public record CheckoutRequest(int CustomerId) : IRequest;

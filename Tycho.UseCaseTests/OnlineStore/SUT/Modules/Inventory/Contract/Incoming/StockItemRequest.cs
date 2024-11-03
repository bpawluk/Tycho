using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;

public record StockItemRequest(int ItemId, uint Quantity) : IRequest;
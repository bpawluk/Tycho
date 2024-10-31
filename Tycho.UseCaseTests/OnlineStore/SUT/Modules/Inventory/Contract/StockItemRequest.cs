using Tycho.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;

public record StockItemRequest(int ItemId, uint Quantity) : IRequest;
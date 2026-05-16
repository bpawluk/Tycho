using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;

public record StockItemRequest(int ItemId, uint Quantity) : IRequest;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Domain;

internal class Reservation(string code, int itemId, uint quantity)
{
    public string Code { get; private set; } = code;

    public int ItemId { get; private set; } = itemId;

    public uint Quantity { get; private set; } = quantity;
}
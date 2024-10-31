namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Domain;

internal class StockLevel(uint quantity, uint version)
{
    public uint Quantity { get; private set; } = quantity;

    public uint Version { get; private set; } = version;

    public void Increase(uint quantity)
    {
        Quantity += quantity;
        Version++;
    }

    public void Decrease(uint quantity)
    {
        Quantity -= quantity;
        Version++;
    }
}

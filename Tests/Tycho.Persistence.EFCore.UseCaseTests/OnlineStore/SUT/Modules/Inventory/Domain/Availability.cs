namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Domain;

internal class Availability(uint quantity, uint version)
{
    public uint Quantity { get; set; } = quantity;

    public uint Version { get; set; } = version;
}

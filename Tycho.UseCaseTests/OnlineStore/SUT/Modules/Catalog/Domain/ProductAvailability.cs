namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;

internal class ProductAvailability(uint quantity, uint version)
{
    public uint Quantity { get; private set; } = quantity;

    public uint Version { get; private set; } = version;
}

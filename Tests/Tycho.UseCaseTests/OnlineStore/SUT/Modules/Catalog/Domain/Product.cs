namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;

internal class Product(string name, decimal price)
{
    public int Id { get; private set; }

    public string Name { get; private set; } = name;

    public decimal Price { get; private set; } = price;

    public ProductAvailability Availability { get; private set; } = new(0, 0);

    public void UpdateAvailability(ProductAvailability newAvailability)
    {
        if (newAvailability.Version > Availability.Version)
        {
            Availability = newAvailability;
        }
    }

    public bool IsEnoughAvailable(uint requestedQuantity)
    {
        return Availability.Quantity >= requestedQuantity;
    }
}
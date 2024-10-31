namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Domain;

internal class Item
{
    public int Id { get; private set; }

    public StockLevel StockLevel { get; private set; }

    public Item() 
    {
        StockLevel = default!;
    }

    public Item(int id, uint quantity)
    {
        Id = id;
        StockLevel = new StockLevel(quantity, 1);
    }

    public void Stock(uint quantity)
    {
        StockLevel.Increase(quantity);
    }
}
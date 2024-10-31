namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;

internal class Basket(int customerId)
{
    public List<BasketItem> _items = [];

    public int Id { get; private set; }

    public int CustomerId { get; private set; } = customerId;

    public bool CheckedOut { get; private set; } = false;

    public decimal TotalAmount { get; private set; } = 0;

    public IReadOnlyList<BasketItem> Items => _items;

    public void Add(BasketItem item)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == item.ProductId);

        if (existingItem is null)
        {
            _items.Add(item);
        }
        else
        {
            existingItem.IncreaseQuantity(item.Quantity);
        }

        TotalAmount += item.Price * item.Quantity;
    }
}
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

    public void ConfirmItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Confirm();
        }
    }

    public void DeclineItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Decline();
        }
    }

    public void Checkout()
    {
        if (Items.All(item => item.Status == BasketItem.ItemStatus.Confirmed))
        {
            CheckedOut = true;
        }
        else
        {
            throw new InvalidOperationException("Tried checking out a basket with unconfirmed items");
        }
    }
}
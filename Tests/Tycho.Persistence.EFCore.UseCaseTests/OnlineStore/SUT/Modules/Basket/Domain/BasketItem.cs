
namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;

internal class BasketItem(int productId, uint quantity, decimal price)
{
    public int ProductId { get; private set; } = productId;

    public uint Quantity { get; private set; } = quantity;

    public decimal Price { get; private set; } = price;

    public ItemStatus Status { get; private set; } = ItemStatus.Pending;

    public void IncreaseQuantity(uint quantity)
    {
        Quantity += quantity;
    }

    internal void Confirm()
    {
        if (Status == ItemStatus.Pending)
        {
            Status = ItemStatus.Confirmed;
        }
    }

    internal void Decline()
    {
        if (Status == ItemStatus.Pending)
        {
            Status = ItemStatus.Declined;
        }
    }

    public enum ItemStatus
    {
        Pending,
        Confirmed,
        Declined
    }
}
namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;

internal class BasketItem(int productId, uint quantity, decimal price)
{
    public int ProductId { get; private set; } = productId;

    public uint Quantity { get; private set; } = quantity;

    public decimal Price { get; private set; } = price;

    public void IncreaseQuantity(uint quantity)
    {
        Quantity += quantity;
    }
}
namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Domain;

internal class Order(int customerId, decimal total)
{
    public int Id { get; private set; }

    public int CustomerId { get; private set; } = customerId;

    public decimal Total { get; private set; } = total;
}
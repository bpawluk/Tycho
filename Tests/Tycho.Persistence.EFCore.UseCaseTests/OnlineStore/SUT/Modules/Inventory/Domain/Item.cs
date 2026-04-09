namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Domain;

internal class Item(int id, uint stockLevel)
{
    public List<Reservation> _reservations = [];

    public int Id { get; private set; } = id;

    public uint StockLevel { get; private set; } = stockLevel;

    public Availability Availability { get; private set; } = new(stockLevel, 1);

    public IReadOnlyList<Reservation> Reservations => _reservations;

    public void Stock(uint quantity)
    {
        StockLevel += quantity;
        RecalculateAvailability();
    }

    public bool Reserve(string reservationCode, uint requestedQuantity)
    {
        if (Reservations.Any(reservation => reservation.Code == reservationCode))
        {
            return true;
        }

        if (Availability.Quantity >= requestedQuantity)
        {
            var newReservation = new Reservation(reservationCode, Id, requestedQuantity);
            _reservations.Add(newReservation);
            RecalculateAvailability();
            return true;
        }

        return false;
    }

    private void RecalculateAvailability()
    {
        var newQuantity = (uint)(StockLevel - Reservations.Sum(r => (int)r.Quantity));
        if (newQuantity != Availability.Quantity)
        {
            Availability = new(newQuantity, ++Availability.Version);
        }
    }
}
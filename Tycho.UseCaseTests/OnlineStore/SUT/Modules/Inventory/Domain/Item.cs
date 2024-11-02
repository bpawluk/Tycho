namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Domain;

internal class Item
{
    public List<Reservation> _reservations = [];

    public int Id { get; private set; }

    public uint StockLevel { get; private set; }

    public IReadOnlyList<Reservation> Reservations => _reservations;

    public Availability Availability { get; private set; } = default!;

    private Item() { }

    public Item(int id, uint initialQuantity)
    {
        Id = id;
        StockLevel = initialQuantity;
        Availability = new(initialQuantity, 1);
    }

    public void Stock(uint quantity)
    {
        StockLevel += quantity;
        RecalculateAvailability();
    }

    public bool Reserve(string reservationCode, uint quantity)
    {
        if (Reservations.Any(reservation => reservation.Code == reservationCode))
        {
            return true;
        }

        if (Availability.Quantity >= quantity)
        {
            var newReservation = new Reservation(reservationCode, Id, quantity);
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
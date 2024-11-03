using Tycho.Events;

namespace Tycho.Persistence.EFCore.UnitTests._Data.Events;

public record TestEventWithData : IEvent
{
    public int Id { get; init; } = 1;
    public string Name { get; init; } = "Sample Name";
    public bool IsActive { get; init; } = true;
    public char Initial { get; init; } = 'A';
    public double Salary { get; init; } = 1000.50;
    public float Height { get; init; } = 5.9f;
    public long Population { get; init; } = 1000000000;

    public int? NullableInt { get; init; } = null;
    public double? NullableDouble { get; init; } = 3.14159;

    public DateTime CreatedDate { get; init; } = new DateTime(2012, 12, 21, 21, 12, 0);
    public DateTime? NullableDate { get; init; } = null;
    public TimeSpan TimeElapsed { get; init; } = TimeSpan.FromHours(1.5);

    public List<string> Tags { get; init; } = ["tag-1", "tag-2", "tag-3"];
    public string[] Nicknames { get; init; } = ["nick-1", "nick-2"];
    public Dictionary<string, int> Scores { get; init; } = new()
    {
        { "Math", 90 },
        { "Science", 85 }
    };

    public StatusType Status { get; init; } = StatusType.Active;

    public Guid UniqueId { get; init; } = new("e9d5fa1c-6f4d-4c89-9783-df28769c8de1");

    public Address Address { get; init; } = new("123 Main St", "Metropolis", "12345");

    public List<Address> PreviousAddresses { get; init; } =
    [
        new("456 Elm St", "Smallville", "67890"),
        new("789 Maple St", "Gotham", "11223")
    ];

    public string GetSerializedPayload() => "{\"Id\":1,\"Name\":\"Sample Name\",\"IsActive\":true,\"Initial\":\"A\",\"Salary\":1000.5,\"Height\":5.9,\"Population\":1000000000,\"NullableInt\":null,\"NullableDouble\":3.14159,\"CreatedDate\":\"2012-12-21T21:12:00\",\"NullableDate\":null,\"TimeElapsed\":\"01:30:00\",\"Tags\":[\"tag-1\",\"tag-2\",\"tag-3\"],\"Nicknames\":[\"nick-1\",\"nick-2\"],\"Scores\":{\"Math\":90,\"Science\":85},\"Status\":0,\"UniqueId\":\"e9d5fa1c-6f4d-4c89-9783-df28769c8de1\",\"Address\":{\"Street\":\"123 Main St\",\"City\":\"Metropolis\",\"ZipCode\":\"12345\"},\"PreviousAddresses\":[{\"Street\":\"456 Elm St\",\"City\":\"Smallville\",\"ZipCode\":\"67890\"},{\"Street\":\"789 Maple St\",\"City\":\"Gotham\",\"ZipCode\":\"11223\"}]}";

    public bool EqualsEvent(TestEventWithData? other)
    {
        if (other is null)
        {
            return false;
        }
        return Id == other.Id &&
               Name == other.Name &&
               IsActive == other.IsActive &&
               Initial == other.Initial &&
               Salary == other.Salary &&
               Height == other.Height &&
               Population == other.Population &&
               NullableInt == other.NullableInt &&
               NullableDouble == other.NullableDouble &&
               CreatedDate == other.CreatedDate &&
               NullableDate == other.NullableDate &&
               TimeElapsed == other.TimeElapsed &&
               Tags.SequenceEqual(other.Tags) &&
               Nicknames.SequenceEqual(other.Nicknames) &&
               Scores.SequenceEqual(other.Scores) &&
               Status == other.Status &&
               UniqueId == other.UniqueId &&
               Address == other.Address &&
               PreviousAddresses.SequenceEqual(other.PreviousAddresses);
    }
};

public enum StatusType
{
    Active,
    Inactive,
    Pending
}

public record Address(string Street, string City, string ZipCode);
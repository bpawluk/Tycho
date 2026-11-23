using Tycho.Persistence.EFCore.Database;
using Tycho.Persistence.EFCore.Entries;

namespace Tycho.Persistence.EFCore.Outbox;

internal sealed class OutboxEntryTable : IEntryTable
{
    public string Name { get; }
    public string OwnerIdColumn { get; }
    public string EntryIdColumn { get; }
    public string StateColumn { get; }
    public string UpdatedColumn { get; }
    public string AttemptsColumn { get; }
    public string ClaimIdColumn { get; }
    public string ClaimExpirationColumn { get; }

    public OutboxEntryTable(TychoDbContext dbContext)
    {
        var table = new TableMapping(dbContext, typeof(OutboxEntry));
        Name = table.Name;
        OwnerIdColumn = table.GetColumn(nameof(OutboxEntry.OwnerId));
        EntryIdColumn = table.GetColumn(nameof(OutboxEntry.EntryId));
        StateColumn = table.GetColumn(nameof(OutboxEntry.State));
        UpdatedColumn = table.GetColumn(nameof(OutboxEntry.Updated));
        AttemptsColumn = table.GetColumn(nameof(OutboxEntry.Attempts));
        ClaimIdColumn = table.GetColumn(nameof(OutboxEntry.ClaimId));
        ClaimExpirationColumn = table.GetColumn(nameof(OutboxEntry.ClaimExpiration));
    }
}

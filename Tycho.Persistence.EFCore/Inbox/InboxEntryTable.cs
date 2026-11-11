using Tycho.Persistence.EFCore.Database;
using Tycho.Persistence.EFCore.Entries;

namespace Tycho.Persistence.EFCore.Inbox;

internal sealed class InboxEntryTable : IEntryTable
{
    public string Name { get; }
    public string OwnerIdColumn { get; }
    public string EntryIdColumn { get; }
    public string StateColumn { get; }
    public string UpdatedColumn { get; }
    public string AttemptsColumn { get; }
    public string ClaimIdColumn { get; }
    public string ClaimExpirationColumn { get; }

    public InboxEntryTable(TychoDbContext dbContext)
    {
        var table = new TableMapping(dbContext, typeof(InboxEntry));
        Name = table.Name;
        OwnerIdColumn = table.GetColumn(nameof(InboxEntry.OwnerId));
        EntryIdColumn = table.GetColumn(nameof(InboxEntry.EntryId));
        StateColumn = table.GetColumn(nameof(InboxEntry.State));
        UpdatedColumn = table.GetColumn(nameof(InboxEntry.Updated));
        AttemptsColumn = table.GetColumn(nameof(InboxEntry.Attempts));
        ClaimIdColumn = table.GetColumn(nameof(InboxEntry.ClaimId));
        ClaimExpirationColumn = table.GetColumn(nameof(InboxEntry.ClaimExpiration));
    }
}

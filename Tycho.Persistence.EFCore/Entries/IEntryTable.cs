namespace Tycho.Persistence.EFCore.Entries;

internal interface IEntryTable
{
    string Name { get; }
    string OwnerIdColumn { get; }
    string EntryIdColumn { get; }
    string StateColumn { get; }
    string UpdatedColumn { get; }
    string AttemptsColumn { get; }
    string ClaimIdColumn { get; }
    string ClaimExpirationColumn { get; }
}

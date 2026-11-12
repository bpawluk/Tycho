using System;

namespace Tycho.Persistence.EFCore.Entries;

internal readonly struct ClaimResult<TEntry> where TEntry : class
{
    public ClaimStatus Status { get; }
    public TEntry? Entry { get; }

    private ClaimResult(ClaimStatus status, TEntry? entry)
    {
        Status = status;
        Entry = entry;
    }

    public static ClaimResult<TEntry> NotClaimed() => new(ClaimStatus.NotClaimed, null);
    public static ClaimResult<TEntry> Claimed(TEntry entry) => new(ClaimStatus.Claimed, entry ?? throw new ArgumentNullException(nameof(entry)));
    public static ClaimResult<TEntry> ClaimedButMissing() => new(ClaimStatus.ClaimedButMissing, null);
}

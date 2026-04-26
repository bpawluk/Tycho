namespace Tycho.Persistence.EFCore.Common;

internal enum EntryState
{
    Failed = -1,
    New = 0,
    InProcessing = 1,
    Processed = 2,
}

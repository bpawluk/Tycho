namespace Tycho.Persistence.EFCore.Outbox;

internal enum EntryState
{
    Failed = -1,
    New = 0,
    InDelivery = 1,
    Delivered = 2,
}

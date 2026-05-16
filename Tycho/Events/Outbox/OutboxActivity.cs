using System;

namespace Tycho.Events.Outbox
{
    internal class OutboxActivity
    {
        public event EventHandler? NewEntriesAdded;

        public void NotifyNewEntriesAdded()
        {
            NewEntriesAdded?.Invoke(this, EventArgs.Empty);
        }
    }
}

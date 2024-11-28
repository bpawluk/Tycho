using System;

namespace Tycho.Persistence
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
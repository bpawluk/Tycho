using System;

namespace Tycho.Events.Inbox
{
    internal class InboxActivity
    {
        public event EventHandler? NewEntriesAdded;

        public void NotifyNewEntriesAdded()
        {
            NewEntriesAdded?.Invoke(this, EventArgs.Empty);
        }
    }
}

using System;
using Microsoft.Extensions.Logging;

namespace Tycho.Events.Inbox
{
    internal class InboxActivity
    {
        private readonly ILogger<InboxActivity>? _logger;

        public InboxActivity(ILogger<InboxActivity>? logger = null)
        {
            _logger = logger;
        }

        public event EventHandler? NewEntriesAdded;

        public void NotifyNewEntriesAdded()
        {
            EventHandler? subscribers = NewEntriesAdded;
            if (subscribers is null)
            {
                return;
            }

            foreach (Delegate subscriber in subscribers.GetInvocationList())
            {
                try
                {
                    ((EventHandler)subscriber)(this, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    TryLogNotificationFailure(exception);
                }
            }
        }

        private void TryLogNotificationFailure(Exception exception)
        {
            try
            {
                _logger?.LogError(exception, "Failed to notify an inbox activity subscriber about new entries.");
            }
            catch
            {
                // Activity notifications must not affect persisted work.
            }
        }
    }
}

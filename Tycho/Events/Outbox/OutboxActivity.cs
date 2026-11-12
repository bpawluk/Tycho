using System;
using Microsoft.Extensions.Logging;
using Tycho.Logging;

namespace Tycho.Events.Outbox
{
    internal class OutboxActivity
    {
        private readonly ILogger<OutboxActivity>? _logger;

        public OutboxActivity(ILogger<OutboxActivity>? logger = null)
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
                _logger?.OutboxNotificationFailed(exception);
            }
            catch
            {
                // Activity notifications must not affect persisted work.
            }
        }
    }
}

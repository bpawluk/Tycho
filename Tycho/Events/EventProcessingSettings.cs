using Tycho.Events.Inbox;
using Tycho.Events.Outbox;
using Tycho.Events.Serialization;

namespace Tycho.Events
{
    /// <summary>
    /// Defines the settings used to process and serialize events in a Tycho host.
    /// </summary>
    public sealed class EventProcessingSettings
    {
        /// <summary>
        /// Gets inbox processor settings.
        /// </summary>
        public InboxSettings Inbox { get; private set; } = new();

        /// <summary>
        /// Gets outbox processor settings.
        /// </summary>
        public OutboxSettings Outbox { get; private set; } = new();

        /// <summary>
        /// Gets JSON payload serializer settings.
        /// </summary>
        public JsonPayloadSerializerSettings PayloadSerializer { get; private set; } = new();

        internal void Validate()
        {
            Inbox.Validate();
            Outbox.Validate();
            PayloadSerializer.Validate();
        }

        internal EventProcessingSettings Copy()
        {
            return new()
            {
                Inbox = Inbox.Copy(),
                Outbox = Outbox.Copy(),
                PayloadSerializer = PayloadSerializer.Copy()
            };
        }
    }
}

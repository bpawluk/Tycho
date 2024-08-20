using Tycho.DependencyInjection;
using Tycho.Messaging;

namespace Tycho.Contract.Inbox.Builder
{
    internal partial class InboxBuilder : IInboxDefinition, IRequestInboxDefinition, IEventInboxDefinition
    {
        private readonly IInstanceCreator _instanceCreator;
        private readonly IMessageRouter _moduleInbox;

        public IEventInboxDefinition Events => this;

        public IRequestInboxDefinition Requests => this;

        public InboxBuilder(IInstanceCreator instanceCreator, IMessageRouter moduleInbox)
        {
            _instanceCreator = instanceCreator;
            _moduleInbox = moduleInbox;
        }

        public IMessageBroker Build() => new MessageBroker(_moduleInbox);
    }
}

using System;

namespace Tycho.Messaging.Forwarders
{
    internal abstract class ForwarderBase<MessageIn, MessageOut>
    {
        protected readonly IModule _target;
        protected readonly Func<MessageIn, MessageOut> _messageMapping;

        protected ForwarderBase(IModule target, Func<MessageIn, MessageOut> mapping)
        {
            _target = target;
            _messageMapping = mapping;
        }
    }
}

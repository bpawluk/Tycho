using System;

namespace Tycho.Messaging.Handlers
{
    internal abstract class ForwardingHandlerBase<MessageIn, MessageOut>
    {
        protected readonly IModule _target;
        protected readonly Func<MessageIn, MessageOut> _mapping;

        protected ForwardingHandlerBase(IModule target, Func<MessageIn, MessageOut> mapping)
        {
            _target = target;
            _mapping = mapping;
        }
    }
}

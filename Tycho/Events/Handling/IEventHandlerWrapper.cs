using System;

namespace Tycho.Events.Handling
{
    internal interface IEventHandlerWrapper
    {
        Type InnerHandlerType { get; }
    }
}

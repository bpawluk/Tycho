using Tycho.Messaging;

namespace Tycho
{
    /// <summary>
    /// An interface that represents a particular module instance <para/>
    /// Use it in your module's code to inject the module's instance into it
    /// </summary>
    public interface IModule : IMessageBroker { }
}

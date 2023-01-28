using Tycho.Messaging;

namespace Tycho
{
    /// <summary>
    /// An interface that represents a module instance
    /// </summary>
    /// <remarks>
    /// Use it in your module's code to inject its instance into it
    /// </remarks>
    public interface IModule : IMessageBroker { }

    /// <summary>
    /// An interface that represents an instance of the specific module type
    /// </summary>
    /// <remarks>
    /// Use it in your code to inject an instance of the required module into it
    /// </remarks>
    /// <typeparam name="Definition">A type that defines the module</typeparam>
    public interface IModule<Definition> : IModule
        where Definition : TychoModule
    { }
}

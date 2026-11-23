using System;
using Tycho.Events.Routing;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Modules.Instance
{
    /// <summary>
    /// Represents a Tycho module instance.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IModule : IRunnable, IDisposable
    {
        internal Internals Internals { get; }

        internal IEventRouter EventRouter { get; }

        internal IRequestBroker RequestBroker { get; }
    }

    /// <summary>
    /// Represents a Tycho module instance defined by <typeparamref name="TTychoDefinition"/>.
    /// </summary>
    /// <typeparam name="TTychoDefinition">The module definition type.</typeparam>
    [ReferencedByReflection]
    [ReferencedBySourceGenerator]
    public interface IModule<TTychoDefinition> : IModule where TTychoDefinition : TychoModule
    {
    }
}

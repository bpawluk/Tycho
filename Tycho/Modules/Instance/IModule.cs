using System;
using Tycho.Events.Broker;
using Tycho.Identity.Modules;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Modules.Instance
{
    /// <summary>
    /// Represents a running Tycho Module instance.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IModule : IAsyncDisposable
    {
        internal ModuleIdentity Identity { get; }

        internal Internals Internals { get; }

        internal IEventBroker EventBroker { get; }

        internal IRequestBroker RequestBroker { get; }
    }

    /// <summary>
    /// Represents a running Tycho Module instance defined by <typeparamref name="TTychoDefinition"/>.
    /// </summary>
    /// <typeparam name="TTychoDefinition">The Module definition type.</typeparam>
    [ReferencedByReflection]
    [ReferencedBySourceGenerator]
    public interface IModule<TTychoDefinition> : IModule
        where TTychoDefinition : TychoModule
    {
    }
}

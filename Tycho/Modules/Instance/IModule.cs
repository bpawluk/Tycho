using System;
using Tycho.Events.Broker;
using Tycho.Identity.Modules;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Modules.Instance
{
    /// <summary>
    /// Represents a running Tycho module instance.
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
    /// Represents a running Tycho module instance defined by <typeparamref name="TTychoDefinition"/>.
    /// </summary>
    /// <typeparam name="TTychoDefinition">The module definition type.</typeparam>
    [ReferencedByReflection]
    [ReferencedBySourceGenerator]
    public interface IModule<TTychoDefinition> : IModule
        where TTychoDefinition : TychoModule
    {
    }
}

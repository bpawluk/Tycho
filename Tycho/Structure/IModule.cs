using System;
using Tycho.Events.Routing;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Structure.Internal;

namespace Tycho.Structure
{
    /// <summary>
    /// Represents a running Tycho module instance.
    /// </summary>
    public interface IModule : IRequestExecutor, IAsyncDisposable
    {
        internal Internals Internals { get; }

        internal IEventRouter EventRouter { get; }
    }

    /// <summary>
    /// Represents a running Tycho module instance defined by <typeparamref name="TTychoDefinition"/>.
    /// </summary>
    /// <typeparam name="TTychoDefinition">The module definition type.</typeparam>
    public interface IModule<TTychoDefinition> : IModule
        where TTychoDefinition : TychoModule
    {
    }
}

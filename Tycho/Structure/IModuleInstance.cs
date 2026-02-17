using System;
using Tycho.Events.Routing;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Structure.Internal;
using Tycho.Utils;

namespace Tycho.Structure
{
    /// <summary>
    /// Represents a running Tycho module instance.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IModuleInstance : IRequestExecutor, IAsyncDisposable
    {
        internal Internals Internals { get; }

        internal IEventRouter EventRouter { get; }
    }

    /// <summary>
    /// Represents a running Tycho module instance defined by <typeparamref name="TTychoDefinition"/>.
    /// </summary>
    /// <typeparam name="TTychoDefinition">The module definition type.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IModuleInstance<TTychoDefinition> : IModuleInstance
        where TTychoDefinition : TychoModule
    {
    }
}

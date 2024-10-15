using Tycho.Events.Routing;
using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.Modules
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IModule : IRequestExecutor
    {
        internal Internals Internals { get; }

        internal IEventRouter EventRouter { get; }
    }

    /// <summary>
    ///     TODO
    /// </summary>
    public interface IModule<TModuleDefinition> : IModule
        where TModuleDefinition : TychoModule
    {
    }
}
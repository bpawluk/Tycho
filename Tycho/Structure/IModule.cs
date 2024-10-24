﻿using Tycho.Events.Routing;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.Structure
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
﻿using TychoV2.Events.Routing;
using TychoV2.Requests;
using TychoV2.Structure;

namespace TychoV2.Modules
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IModule : IExecute
    {
        internal Internals Internals { get; }

        internal EventRouter EventRouter { get; }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public interface IModule<TModuleDefinition> : IModule
        where TModuleDefinition : TychoModule
    {
    }
}
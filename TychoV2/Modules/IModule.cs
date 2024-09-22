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
    }

    /// <summary>
    /// TODO
    /// </summary>
    public interface IModule<TModuleDefinition> : IModule
        where TModuleDefinition : TychoModule
    {
    }
}
using TychoV2.Requests;
using TychoV2.Structure;

namespace TychoV2.Modules
{
    public interface IModule : IExecute
    {
        internal Internals Internals { get; }
    }

    public interface IModule<TModuleDefinition> : IModule
        where TModuleDefinition : TychoModule
    {
    }
}
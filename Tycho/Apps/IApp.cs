using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.Apps
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IApp : IRequestExecutor
    {
        internal Internals Internals { get; }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public interface IApp<TAppDefinition> : IApp
        where TAppDefinition : TychoApp
    {
    }
}
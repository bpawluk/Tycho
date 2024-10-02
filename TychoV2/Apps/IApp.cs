using TychoV2.Requests;
using TychoV2.Structure;

namespace TychoV2.Apps
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IApp : IExecute
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
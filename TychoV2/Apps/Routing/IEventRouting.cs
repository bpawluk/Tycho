using TychoV2.Modules;

namespace TychoV2.Apps.Routing
{
    public interface IEventRouting
    {
        /// <summary>
        /// TODO
        /// </summary>
        IEventRouting Forwards<Module>()
            where Module : TychoModule;
    }
}

using TychoV2.Modules;

namespace TychoV2.Apps.Routing
{
    public interface IEventRouting
    {
        /// <summary>
        /// TODO
        /// </summary>
        IEventRouting Forward<Module>()
            where Module : TychoModule;
    }
}

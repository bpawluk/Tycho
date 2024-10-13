using Tycho.Modules;

namespace Tycho.Apps.Routing
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

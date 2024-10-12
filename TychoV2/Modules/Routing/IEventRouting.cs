namespace TychoV2.Modules.Routing
{
    public interface IEventRouting
    {
        /// <summary>
        /// TODO
        /// </summary>
        IEventRouting Exposes();

        /// <summary>
        /// TODO
        /// </summary>
        IEventRouting Forwards<Module>()
            where Module : TychoModule;
    }
}

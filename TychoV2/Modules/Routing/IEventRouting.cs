namespace TychoV2.Modules.Routing
{
    public interface IEventRouting
    {
        /// <summary>
        /// TODO
        /// </summary>
        IEventRouting Expose();

        /// <summary>
        /// TODO
        /// </summary>
        IEventRouting Forward<Module>()
            where Module : TychoModule;
    }
}

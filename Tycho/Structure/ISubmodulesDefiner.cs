using System;
using Tycho.Messaging.Contracts;

namespace Tycho.Structure
{
    public interface ISubmodulesDefiner
    {
        ISubmodulesDefiner Add<Module>(Action<IOutboxConsumer> contractFullfilment)
            where Module : ModuleDefinition;

        ISubmodulesDefiner Add<Module>(Action<IOutboxConsumer, IServiceProvider> contractFullfilment)
            where Module : ModuleDefinition;
    }
}

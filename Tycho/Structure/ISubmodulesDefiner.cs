using System;
using Tycho.Messaging.Contracts;

namespace Tycho.Structure
{
    public interface ISubmodulesDefiner
    {
        ISubmodulesDefiner Add<Module>(Action<IOutboxConsumer> contractFullfilment)
            where Module : ModuleDefinition, new();
    }
}

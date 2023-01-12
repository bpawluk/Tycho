using System;
using Tycho.Messaging.Contracts;

namespace Tycho.Structure;

internal class SubmodulesBuilder : ISubmodulesDefiner, ISubmodulesBuilder
{
    public ISubmodulesDefiner Add<Module>(Action<IOutboxConsumer> contractFullfilment) 
        where Module : ModuleDefinition
    {
        throw new NotImplementedException();
    }

    public ISubmodulesDefiner Add<Module>(Action<IOutboxConsumer, IServiceProvider> contractFullfilment) 
        where Module : ModuleDefinition
    {
        throw new NotImplementedException();
    }
}

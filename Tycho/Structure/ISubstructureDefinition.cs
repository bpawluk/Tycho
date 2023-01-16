using System;
using Tycho.Contract;

namespace Tycho.Structure
{
    public interface ISubstructureDefinition
    {
        ISubstructureDefinition AddSubmodule<Module>()
            where Module : TychoModule, new();

        ISubstructureDefinition AddSubmodule<Module>(Action<IOutboxConsumer> consumeMessages)
            where Module : TychoModule, new();
    }
}
